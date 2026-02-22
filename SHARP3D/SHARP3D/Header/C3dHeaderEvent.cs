using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Header
{
    // TODO: Implement methods to curate the eventLabel. Don't know where to put it.
    /// <summary>
    /// Represents an event in a C3D file header, including its time, display flag, and label.
    /// </summary>
    public struct C3dHeaderEvent
    {
        /// <summary>
        /// The time at which the event occurs.
        /// </summary>
        public float EventTime;
        /// <summary>
        /// Specifies the <see cref="HeaderEventFlag"/> for the event. 
        /// </summary>
        public HeaderEventFlag DisplayFlag;
        /// <summary>
        /// Specifies the label associated with the event. It is either a 2 or 4 character string of ASCII character depending on the C3D file version.
        /// </summary>
        public string EventLabel;

        public static C3dHeaderEvent[] EventsFromBinaries(byte[] binaries, int definedEventsNb, bool supported4CharLabels, ProcessorType processorFile)
        {
            C3dHeaderEvent[] events = new C3dHeaderEvent[definedEventsNb];
            for (int i = 0; i < definedEventsNb; i++)
            {
                events[i] = EventFromBinaries(
                    binaries.Skip(4 * i).Take(4).ToArray(),
                    binaries.Skip(1 * i + 72).Take(1).ToArray(),
                    supported4CharLabels ? binaries.Skip(i * 4 + 92).Take(4).ToArray() : binaries.Skip(i * 2 + 92).Take(2).ToArray(),
                    supported4CharLabels,
                    processorFile
                    );
            }
            return events;
        }

        public static C3dHeaderEvent EventFromBinaries(
            byte[] binEventTime,
            byte[] binHeaderEventFlag,
            byte[] binEventLabel,
            bool supported4CharLabels,
            ProcessorType processorFile
            )
        {
            C3dHeaderEvent headerEvent = new C3dHeaderEvent();
            headerEvent.EventTime = C3dBytesConvertor.ToFloat(binEventTime, processorFile);
            headerEvent.DisplayFlag = HeaderEventFlagHelper.FromByte(binHeaderEventFlag[0]);
            // Assuming the label is a fixed length string of 16 bytes
            headerEvent.EventLabel = System.Text.Encoding.ASCII.GetString(binEventLabel, 0, supported4CharLabels?4:2).TrimEnd('\0');
            return headerEvent;
        }

        
    }

    
}