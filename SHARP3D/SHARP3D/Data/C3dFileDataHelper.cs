using SHARP3D.Data.DataEntity;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System;


namespace SHARP3D.Data
{
    /// <summary>
    /// Provides helper methods for reading, parsing, and processing C3D file data.
    /// </summary>
    /// <remarks>
    /// This class contains utility methods to simplify common operations on C3D files,
    /// such as reading data frames, processing points and analogs, and validating data integrity.
    /// </remarks>
    public static class C3dFileDataHelper
    {
        /// <summary>
        /// Reads and parses C3D data from a file stream using the provided context.
        /// </summary>
        /// <param name="context">The <see cref="C3dFileDataContext"/> containing file stream and metadata.</param>
        /// <returns>A <see cref="C3dFileData"/> object containing parsed points and analogs. And an int containing the ANALOG:BITS guesstimate.</returns>
        /// <exception cref="EndOfStreamException">
        /// Many of the file given by the C3D organization as example to test any C3D app present too little data for the amount of frame advertised in regards to their parameters value. 
        /// Due to the fact that they still showcase valid data till the cutoff, we decided to provide our library with a non punitive approach: this function will read frames from a C3D file till it either reach the number of frame advertised (best case, and what should be the norm) or reach the end of the stream (worst case).
        /// That said we do not approve of the practice of cutting off mid-frames.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the data type is not supported (neither INT16 nor FLOAT32).
        /// </exception>
        public static (C3dFileData, int) FromFileStream(C3dFileDataContext context) 
        {
            // TODO: Add the check for AnalogSamplePerFrame, total number of analog sample must be a mutliple of this. In the way the c3d file is done there is a better way to check for that I think
            context.C3dStream.Seek(context.PointerDataSection, SeekOrigin.Begin);

            List<C3dFileDataPoint[]> points = new List<C3dFileDataPoint[]>();
            List<float[][]> analogs = new List<float[][]>();
            int maxAnalogSample = 0;

            for (int i = 0; i < context.FramesNumber; i++)
            {
                try
                {
                    (C3dFileDataPoint[], float[][], int) frame;
                    switch (context.DataTypeFile)
                    {
                        case DataType.INT16:
                            frame = ReadDataFrameInt16(context);
                            break;
                        case DataType.FLOAT32:
                            frame = ReadDataFrameFloat32(context);
                            break;
                        default:
                            throw new NotSupportedException("The C3D file data is neither stored in a supported format: INT16 or FLOAT32.");
                    }
                    points.Add(frame.Item1);
                    analogs.Add(frame.Item2);

                    if (maxAnalogSample < Math.Abs(frame.Item3))
                    {
                        maxAnalogSample = frame.Item3;
                    }
                }
                catch (EndOfStreamException e)
                {
                    // Support file that are missing data compared to the advertised number of frame and parameter values.
                    // There is just too many of those files in the examples to just discard them.
                    Console.Error.WriteLine($"WARNING: the file does not contains enough data for the expected frame number. It is missing {context.FramesNumber - i} frames.");
                    break;
                }
            }

            // Compute the guesstimate ANALOG:BITS value.
            // This is not a foolproof solution, but it is as best as it will get.
            //Unsigned Integers

            //    12 - bit: 0 to 4,095
            //    13 - bit: 0 to 8,191
            //    14 - bit: 0 to 16,383
            //    15 - bit: 0 to 32,767
            //    16 - bit: 0 to 65,535

            //Signed Integers(Two's Complement)

            //    12 - bit: -2,048 to 2,047
            //    13 - bit: -4,096 to 4,095
            //    14 - bit: -8,192 to 8,191
            //    15 - bit: -16,384 to 16,383
            //    16 - bit: -32,768 to 32,767

            // https://electronics.stackexchange.com/a/163236
            // Due to this we skip the odd bit ADC, for the moment at least. Because 13 bits ADC get flags by our algorithm too often to be normal.
            int analogBitsGuesstimate = 12;
            switch (context.DataTypeFile)
            {
                case DataType.INT16:
                    switch (context.AnalogFormat)
                    {
                        case AnalogFormatFlag.UNSIGNED:
                            if (maxAnalogSample <= 4095)
                            {
                                analogBitsGuesstimate = 12;
                            }
                            //else if (maxAnalogSample <= 8191)
                            //{
                            //    analogBitsGuesstimate = 13;
                            //}
                            else if (maxAnalogSample <= 16383)
                            {
                                analogBitsGuesstimate = 14;
                            }
                            //else if (maxAnalogSample <= 32767)
                            //{
                            //    analogBitsGuesstimate = 15;
                            //}
                            else if (maxAnalogSample <= 65535)
                            {
                                analogBitsGuesstimate = 16;
                            }
                            break;
                        case AnalogFormatFlag.SIGNED:
                            if (maxAnalogSample <= 2047)
                            {
                                analogBitsGuesstimate = 12;
                            }
                            //else if (maxAnalogSample <= 4095)
                            //{
                            //    analogBitsGuesstimate = 13;
                            //}
                            else if (maxAnalogSample <= 8191)
                            {
                                analogBitsGuesstimate = 14;
                            }
                            //else if (maxAnalogSample <= 16383)
                            //{
                            //    analogBitsGuesstimate = 15;
                            //}
                            else if (maxAnalogSample <= 32767)
                            {
                                analogBitsGuesstimate = 16;
                            }
                            break;
                    }
                    break;
                case DataType.FLOAT32:
                    switch (maxAnalogSample)
                    {
                        case >= 0:
                            if (maxAnalogSample <= 4095)
                            {
                                analogBitsGuesstimate = 12;
                            }
                            //else if (maxAnalogSample <= 8191)
                            //{
                            //    analogBitsGuesstimate = 13;
                            //}
                            else if (maxAnalogSample <= 16383)
                            {
                                analogBitsGuesstimate = 14;
                            }
                            //else if (maxAnalogSample <= 32767)
                            //{
                            //    analogBitsGuesstimate = 15;
                            //}
                            else if (maxAnalogSample <= 65535)
                            {
                                analogBitsGuesstimate = 16;
                            }
                            break;

                        case < 0:
                            if (maxAnalogSample <= 2047)
                            {
                                analogBitsGuesstimate = 12;
                            }
                            //else if (maxAnalogSample <= 4095)
                            //{
                            //    analogBitsGuesstimate = 13;
                            //}
                            else if (maxAnalogSample <= 8191)
                            {
                                analogBitsGuesstimate = 14;
                            }
                            //else if (maxAnalogSample <= 16383)
                            //{
                            //    analogBitsGuesstimate = 15;
                            //}
                            else if (maxAnalogSample <= 32767)
                            {
                                analogBitsGuesstimate = 16;
                            }
                            break;
                    }
                    break;
            }

            return (
                new C3dFileData
                    {
                        Points = points,
                        Analogs = analogs
                },
                analogBitsGuesstimate
                );
        }

        /// <summary>
        /// Reads a data frame with INT16 data type.
        /// </summary>
        /// <param name="context">The <see cref="C3dFileDataContext"/> containing file stream and metadata.</param>
        /// <returns>A tuple containing arrays of points and analogs for the frame and maximum value of the raw sample.</returns>
        internal static (C3dFileDataPoint[], float[][], int) ReadDataFrameInt16(C3dFileDataContext context) 
        {
            
            List<C3dFileDataPoint> points = new List<C3dFileDataPoint>();
            List<float[]> analogs = new List<float[]>();
            // This is used to compute ANALOG:BITS. 
            // We need the maximum value of raw analog sample to "guesstimate" the bit resolution of the ADC used for acquistion.
            int maxRawAnalogSample = 0;

            // Get POINTS
            for (int i = 0; i < context.MarkersPerFrame; i++)
            {
                List<float> pointValues = new List<float>();
                for (int j=0; j < 3; j++)
                { 
                    byte[] buffer = new byte[2];
                    context.C3dStream.ReadExactly(buffer);
                    pointValues.Add(C3dBytesConvertor.ToInt(buffer, context.Processor) * context.PointScaleFactor);
                }
                byte[] bufferCamSignRes = new byte[2];
                context.C3dStream.ReadExactly(bufferCamSignRes);

                Int16 valueCamSignRes = (Int16) C3dBytesConvertor.ToInt(bufferCamSignRes, context.Processor);
                byte[] CamSignResBytesArray = BitConverter.GetBytes(valueCamSignRes);
                byte camAndSign = CamSignResBytesArray[1];
                int residualInt = CamSignResBytesArray[0];

                bool[] cameraMask = GetCameraMask(camAndSign);

                points.Add(new C3dFileDataPoint 
                {
                    Point = pointValues.ToArray(),
                    AverageResidual = residualInt * context.PointScaleFactor,
                    CameraMask = cameraMask,
                    Raw = IsRaw(camAndSign, residualInt),
                    Valid = IsValid(camAndSign)
                });
            }
            // Get Analogs
            bool isThereNegativeValues = false;
            for (int i = 0; i < context.AnalogSamplePerFrame; i++)
            {
                float[] oneFullAnalogsSample = new float[context.AnalogChannels];
                for (int j = 0; j < context.AnalogChannels; j++)
                {
                    byte[] buffer = new byte[2];
                    context.C3dStream.ReadExactly(buffer);
                    
                    int rawAnalogSample;
                    // As per page 58 of the C3D User Guide
                    switch (context.AnalogFormat)
                    {
                        case AnalogFormatFlag.UNSIGNED:
                            rawAnalogSample = C3dBytesConvertor.ToUInt(buffer, context.Processor);
                            break;
                        default:
                            rawAnalogSample = C3dBytesConvertor.ToInt(buffer, context.Processor);
                            break;
                    }
                    
                    oneFullAnalogsSample[j] = (rawAnalogSample - context.AnalogOffset[j]) * context.AnalogChannelScaleFactor[j] * context.AnalogGeneralScaleFactor;
                    // WARNING POSSIBLITY OF BUFFER OVERFLOW
                    if (maxRawAnalogSample < Math.Abs(rawAnalogSample))
                    {
                        maxRawAnalogSample = rawAnalogSample;
                    }
                    if (rawAnalogSample < 0)
                    {
                        isThereNegativeValues = true;
                    }
                }
                analogs.Add(oneFullAnalogsSample);
            }
            if (isThereNegativeValues)
            {
                maxRawAnalogSample = -maxRawAnalogSample;
            }
            return (points.ToArray(), analogs.ToArray(), maxRawAnalogSample);
        }

        /// <summary>
        /// Reads a data frame with FLOAT32 data type.
        /// </summary>
        /// <param name="context">The <see cref="C3dFileDataContext"/> containing file stream and metadata.</param>
        /// <returns>A tuple containing arrays of points and analogs for the frame.</returns>
        internal static (C3dFileDataPoint[], float[][], int) ReadDataFrameFloat32(C3dFileDataContext context) 
        {
            
            List<C3dFileDataPoint> points = new List<C3dFileDataPoint>();
            List<float[]> analogs = new List<float[]>();
            // This is used to compute ANALOG:BITS. 
            // We need the maximum value of raw analog sample to "guesstimate" the bit resolution of the ADC used for acquistion.
            int maxRawAnalogSample = 0;

            // Get POINTS

            for (int i = 0; i < context.MarkersPerFrame; i++)
            {
                List<float> pointValues = new List<float>();
                for (int j = 0; j < 3; j++)
                {
                    byte[] buffer = new byte[4];
                    context.C3dStream.ReadExactly(buffer);
                    pointValues.Add(C3dBytesConvertor.ToFloat(buffer, context.Processor));
                }
                byte[] floatCamSignResidualBuffer = new byte[4];
                context.C3dStream.ReadExactly(floatCamSignResidualBuffer);
                // TODO: Handle out of range value but that shouldn't happen
                // According to page 51 of th C3D User Guide, you have to convert the float32 to a signed int16. 
                // We therefore don't expect any value out of bond, but just in case we will put a warning
                float floatCamSignResidual = C3dBytesConvertor.ToFloat(floatCamSignResidualBuffer, context.Processor);
                
                // TODO: Reactivate for publication. Too many files badly done which create way too much warnings.
                // It seems that it is done on purpose though.
                if (floatCamSignResidual > 32768)
                {
                    if (floatCamSignResidual > 65536)
                    {
                        //Console.WriteLine($"WARNING: Camera mask and Residual Float32 value overflowed the Int16 format. Value: {floatCamSignResidual}");
                    }
                    else
                    {
                        //Console.WriteLine($"WARNING: Camera mask and Residual Float32 value was above the signed limit of Int16 format. Value: {floatCamSignResidual}");
                    }    
                }
                byte[] intCamSignResidual = BitConverter.GetBytes((Int16)(int)floatCamSignResidual);
                byte camAndSign = intCamSignResidual[1];
                int residualInt = intCamSignResidual[0];

                // Because of Codamotion that invert the bytes order of Word 4 of the C3D data frame.
                //if (context.Software == C3dSoftware.CODAMOTION) 
                //{
                //    byte tempSwitch = camAndSign;
                //    camAndSign = (byte)residualInt;
                //    residualInt = tempSwitch;
                //}
                bool[] cameraMask = GetCameraMask(camAndSign);
                // Cf C3d.Save()
                // That way we only provide what is actually viable precision of the residual
                // and don't induce any false sense of increased precision with the many decimal due to the multiplication by ScaleFactor.
                // It also help "not corrupting" the residual data. As it still corrupt it on the first read, due to the limitation of precision imposed by SHARP3D 
                int decimalPlaces = Math.Max(0, -(int)Math.Ceiling(Math.Log10(context.PointScaleFactor)));
                //double result = Math.Round((double)trajectory.Residual[idFrame], decimalPlaces);
                float averageResidual = (float)Math.Round((float)residualInt * context.PointScaleFactor, decimalPlaces);
                points.Add(new C3dFileDataPoint
                {
                    Point = pointValues.ToArray(),
                    AverageResidual = averageResidual,
                    CameraMask = cameraMask,
                    Raw = IsRaw(camAndSign, residualInt),
                    Valid = IsValid(camAndSign)
                });
            }
            // Get Analogs
            float rawAnalogSampleFloat;
            int rawAnalogSampleInt;
            bool isThereNegativeValues = false;
            for (int i = 0; i < context.AnalogSamplePerFrame; i++)
            {
                float[] oneFullAnalogsSample = new float[context.AnalogChannels];
                for (int j = 0; j < context.AnalogChannels; j++)
                {
                    byte[] buffer = new byte[4];
                    context.C3dStream.ReadExactly(buffer);
                    rawAnalogSampleFloat = C3dBytesConvertor.ToFloat(buffer, context.Processor);
                    // I can't remember why I am doing this line below
                    rawAnalogSampleInt = (int)(rawAnalogSampleFloat > 0 ? Math.Ceiling(rawAnalogSampleFloat) : Math.Floor(rawAnalogSampleFloat));
                    oneFullAnalogsSample[j] = (rawAnalogSampleFloat - context.AnalogOffset[j]) * context.AnalogChannelScaleFactor[j] * context.AnalogGeneralScaleFactor;

                    // WARNING POSSIBLITY OF BUFFER OVERFLOW
                    try
                    {
                        if (!float.IsNaN(rawAnalogSampleFloat)
                            && maxRawAnalogSample < Math.Abs(rawAnalogSampleInt)
                            )
                        {
                            maxRawAnalogSample = Math.Abs(rawAnalogSampleInt);
                        }
                    }
                    catch(OverflowException ex)
                    {
                        // Buffer overflow
                        Console.WriteLine($"Buffer overflow while reading analog value from channel index {i}.");
                    }
                    
                    if (rawAnalogSampleFloat < 0)
                    {
                        isThereNegativeValues = true;
                    }
                }
                //analogValues.Add(oneFullAnalogsSample);
                analogs.Add(oneFullAnalogsSample);
            }
            // This is done so that we take into account if there are negatives value. If there are, that would impact the potential ADC range,
            // and therefore the ANALOG:BITS guesstimate.
            if (isThereNegativeValues)
            {
                maxRawAnalogSample = -maxRawAnalogSample;
            }
            // Then I think I can just return the list<float> as array for analog and the List<C3dDataPoint> and get going.
            return (points.ToArray(), analogs.ToArray(), maxRawAnalogSample);
        }

        /// <summary>
        /// Determines if a point is raw or interpolated based on camera/sign byte and residual.
        /// </summary>
        /// <param name="camAndSign">The camera and sign byte.</param>
        /// <param name="residual">The residual value.</param>
        /// <returns>True if the point is raw; otherwise, false.</returns>
        internal static bool IsRaw(byte camAndSign, int residual)
        {
            if (camAndSign == 0b00000001 || residual == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        internal static bool IsValid(byte camSign)
        {
            sbyte signedCamSign = (sbyte)camSign;
            return signedCamSign < 0 ? false : true;
        }

        internal static bool[] GetCameraMask(byte camAndSign) 
        {
            bool[] cameraMask = new bool[7];
            for (int i = 0; i < 7; i++) // Loop through all 8 bits
            {
                int mask = 1 << i; // Create a mask for the i-th bit
                cameraMask[i] = (camAndSign & mask) != 0 ? true : false; // Check if the bit is set
            }
            return cameraMask;
        }
    }
}
