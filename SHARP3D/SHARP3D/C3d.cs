using SHARP3D.Data.Clean;
using SHARP3D.Header.DataEntity;
using SHARP3D.Parameter.DataEntity.Clean;
using SHARP3D.Utils.Enum;

namespace SHARP3D
{
    public class C3d
    {
        public string? FilePath = null;

        public C3dFileHeaderEvent[] HeaderEvents = new C3dFileHeaderEvent[] { };

        public C3dRequiredParameters Required = new C3dRequiredParameters();

        public C3dParameterSection Parameters = new C3dParameterSection();

        public C3dData Data = new C3dData();

        public C3d() { }

        public C3d(C3dData data, C3dParameterSection? parameters = null, C3dRequiredParameters? required = null)
        {
            Data = data;
            Required = required != null ? required : new C3dRequiredParameters(); 
            Parameters = parameters != null ? parameters : new C3dParameterSection();
        }

        public C3d(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            FilePath = c3dFile.FilePath;

            HeaderEvents = c3dFile.Header.Events;

            Required.Point = new C3dParameterPoint
            {
                Rate = c3dFile.Point.Rate,
                Units = c3dFile.Point.Units,
                Frames = c3dFile.Point.Frames,
                MaximumInterpolationGap = c3dFile.Header.MaxFrameIntepolationGap
            };
            Required.Analog = new C3dParameterAnalog{
                GeneralScale = c3dFile.Analog.GeneralScale,
                SamplesPerFrame = c3dFile.Analog.SamplesPerFrame,
            };

            Parameters = new C3dParameterSection(c3dFile.Parameters);

            Data = new C3dData(c3dFile);

            Parameters.DeleteUneededParametersFromFiles();
        }

        public void ChangePointUnit(string newUnit, float factor)
        {
            Required.Point.Units = newUnit;
            foreach(C3dPointTrajectory trajectory in Data.Points)
            {
                for (int i = 0; i < trajectory.Point.GetLength(0); i++)
                {
                    for(int j=0; j< trajectory.Point.GetLength(1); j++)
                    {
                        trajectory.Point[i,j] = trajectory.Point[i,j] * factor;
                    }
                    trajectory.Residual[i] = trajectory.Residual[i] * factor;
                }
            }
            
        }

        public void ChangeAnalogGeneralScale(float generalScale)
        {
            foreach(C3dAnalogChannel channel in Data.Analogs)
            {
                for(int i=0; i < channel.Data.Length; i++)
                {
                    channel.Data[i] = channel.Data[i] / Required.Analog.GeneralScale * generalScale;
                }
            }

            for(int idPlate = 0; idPlate<Data.Forceplates.Length;idPlate++)
            {
                foreach (C3dAnalogChannel channel in Data.Forceplates[idPlate].Channels)
                {
                    for (int i = 0; i < channel.Data.Length; i++)
                    {
                        channel.Data[i] = channel.Data[i] / Required.Analog.GeneralScale * generalScale;
                    }
                }
            }

            Required.Analog.GeneralScale = generalScale;    
        }

        

        // TODO
        public int AddFrame()
        {
            return Data.Points.Length != 0 ? Data.Points[0].Residual.Length : 0;
        }

        // TODO
        public int DeleteFrame(int idFrameToDelete)
        {
            foreach (C3dPointTrajectory trajectory in Data.Points)
            {

            }

            foreach (C3dAnalogChannel channel in Data.Analogs)
            {

            }

            foreach (C3dForceplate forcePlate in Data.Forceplates)
            {

            }

            return Data.Points.Length != 0 ? Data.Points[0].Residual.Length : 0;
        }

        public void Save(string filePath, FileFrameFormat fileFrameFormat = FileFrameFormat.C3D_STANDARD)
        {
            byte[] parameters = ParametersToBinaries();
            byte[] header = HeaderToBinaries(parameters.Length / 512);
            byte[] data = DataToBinaries();
            byte[] c3dBinaries = header.Concat(parameters).Concat(data).ToArray();
        }

        public byte[] ParametersToBinaries()
        {
            List<byte> parameters = new List<byte>();
            // Introduction to the parameter section
            parameters.Add((byte)0x00);// Unused/Ignored/Reserved
            parameters.Add((byte)0x00);// Unused/Ignored/Reserved
            parameters.Add((byte)0x00);// Length
            parameters.Add(BitConverter.IsLittleEndian? (byte)0x54 : (byte)0x56);// Processor. Not sure if SIG/MIPS are a general big Endian. But that's the closest.


            // Put the length of the parameter section in the third byte
            parameters[2] = (byte)(parameters.Count / 512 + 1);
            return parameters.ToArray();
        }

        public byte[] DataToBinaries(float scaleFactor)
        {
            List<byte> data = new List<byte>();

            return data.ToArray();
        }

        public byte[] HeaderToBinaries(int blockLengthParameterSection)
        {
            List<byte> header = new List<byte>();
            //   Byte 1: uint8, Byte 2: char Byte 1: Number of 512 - byte blocks to Parameter Section +1.
            header.Add((byte)0x02);
            //    Byte 2: Data storage format flag.
            header.Add((byte)0x50);
            //2   uint16 Number of markers stored in each Data Frame.
            header.AddRange(BitConverter.GetBytes((UInt16)Data.Points.Length));
            //3   uint16 Total number of analog samples per Data Frame.
            header.AddRange(BitConverter.GetBytes((UInt16)(Required.Analog.SamplesPerFrame * Data.Analogs.Length)));
            //4   uint16 First frame number of raw data(not used / misleading).
            header.AddRange(BitConverter.GetBytes((UInt16)1));
            //5   uint16 Last frame number of raw data(not used / misleading).
            header.AddRange(BitConverter.GetBytes((UInt16)Required.Point.Frames));
            //6   uint16 Maximum 3D frame interpolation gap.
            header.AddRange(BitConverter.GetBytes((UInt16)Required.Point.MaximumInterpolationGap));
            //7 - 8     float32 Data Scale factor.
            float maximumValue = 0f;
            (float?[,,] allPointsData, _, _) = Data.GetAllPointsData();
            foreach(float? point in allPointsData)
            {
                if (point.HasValue && Math.Abs(point.Value) > maximumValue)
                {
                    maximumValue = Math.Abs(point.Value);
                }
            }
            header.AddRange(BitConverter.GetBytes(maximumValue / 32000f));
            //9   uint16 Number of 512 - byte blocks to the Data Section + 1.
            header.AddRange(BitConverter.GetBytes((UInt16)(blockLengthParameterSection + 1)));
            //10  uint16 Analog Frames per Data Frame.
            header.AddRange(BitConverter.GetBytes((UInt16)Required.Analog.SamplesPerFrame));
            //11 - 12   float32     3D Point Data acquisition rate in Hertz.
            header.AddRange(BitConverter.GetBytes(Required.Point.Rate * (float)Required.Analog.SamplesPerFrame));
            //13 - 149  — 	Not used.
            for(int i = 0; i < 137; i++)
            {
                header.AddRange(BitConverter.GetBytes((UInt16)0));
            }
            //150     uint16 Indicates support for 2 or 4 - character Header Event labels.
            header.AddRange(BitConverter.GetBytes((UInt16)12345));
            //151     uint16  Number of Header Events(0 - 18).
            header.AddRange(BitConverter.GetBytes((UInt16)HeaderEvents.Length));
            //152 	— 	Not used.
            header.AddRange(BitConverter.GetBytes((UInt16)0));
            //153 - 188     float32     Header Event times in seconds.
            for (int i = 0; i < 18; i++)
            {
                if (i < HeaderEvents.Length)
                {
                    header.AddRange(BitConverter.GetBytes(HeaderEvents[i].EventTime));
                }
                else
                {
                    header.AddRange(BitConverter.GetBytes(0f));
                }
            }
            //189 - 197     uint8   Header Event flag(0x00 = ON, 0x01 = OFF).
            for (int i = 0; i < 18; i++)
            {
                if (i < HeaderEvents.Length)
                {
                    header.Add((byte)HeaderEvents[i].DisplayFlag);
                }
                else
                {
                    header.Add((byte)0x00);
                }
            }
            //198 	— 	Not used.
            header.AddRange(BitConverter.GetBytes((UInt16)0));
            //199 - 234     ASCII   Header Event labels(2 or 4 characters, depending on Word 150).
            for (int i = 0; i < 18; i++)
            {
                if (i < HeaderEvents.Length)
                {
                    string label = HeaderEvents[i].EventLabel;
                    if (label.Length > 4)
                    {
                        label = label.Substring(0, 4);
                    }
                    else if (label.Length < 4)
                    {
                        label = label.PadRight(4, '\0');
                    }
                    header.AddRange(System.Text.Encoding.ASCII.GetBytes(label));
                }
                else
                {
                    header.AddRange(System.Text.Encoding.ASCII.GetBytes("\0\0\0\0"));
                }
            }
            //235 - 256 	— 	Not used.
            for(int i = 0; i < 22; i++)
            {
                header.AddRange(BitConverter.GetBytes((UInt16)0));
            }
            return header.ToArray();
        }
    }
}
