using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Header.DataEntity
{
    // TODO: Implement methods to curate the eventLabel. Don't know where to put it.
    /// <summary>
    /// Represents an event in a C3D file header, including its time, display flag, and label.
    /// </summary>
    public struct C3dFileHeaderEvent : IEquatable<C3dFileHeaderEvent>
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

        /// <summary>
        /// Determines whether the current <see cref="C3dFileHeaderEvent"/> instance is equal to another <see cref="C3dFileHeaderEvent"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="C3dFileHeaderEvent"/> instance to compare with the current instance.</param>
        /// <returns>True if the current instance is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        public bool Equals(C3dFileHeaderEvent other)
        {
            return EventTime == other.EventTime &&
                   DisplayFlag == other.DisplayFlag &&
                   string.Equals(EventLabel, other.EventLabel);
        }

        /// <summary>
        /// Determines whether the current <see cref="C3dFileHeaderEvent"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if the current instance is equal to the <paramref name="obj"/> parameter; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is C3dFileHeaderEvent other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="C3dFileHeaderEvent"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + EventTime.GetHashCode();
                hash = hash * 23 + DisplayFlag.GetHashCode();
                hash = hash * 23 + (EventLabel?.GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <summary>
        /// Determines whether two specified <see cref="C3dFileHeaderEvent"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="C3dFileHeaderEvent"/> instance to compare.</param>
        /// <param name="right">The second <see cref="C3dFileHeaderEvent"/> instance to compare.</param>
        /// <returns>True if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(C3dFileHeaderEvent left, C3dFileHeaderEvent right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="C3dFileHeaderEvent"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="C3dFileHeaderEvent"/> instance to compare.</param>
        /// <param name="right">The second <see cref="C3dFileHeaderEvent"/> instance to compare.</param>
        /// <returns>True if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(C3dFileHeaderEvent left, C3dFileHeaderEvent right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Parses a byte array into an array of <see cref="C3dFileHeaderEvent"/> instances.
        /// </summary>
        /// <param name="binaries">The byte array to parse.</param>
        /// <param name="definedEventsNb">The number of events defined in the byte array.</param>
        /// <param name="supported4CharLabels">Indicates whether the C3D file supports 4-character event labels.</param>
        /// <param name="processorFile">The <see cref="ProcessorType"/> used to interpret the byte order of the binary data.</param>
        /// <returns>An array of <see cref="C3dFileHeaderEvent"/> instances populated with the parsed data.</returns>
        public static C3dFileHeaderEvent[] EventsFromBinaries(byte[] binaries, int definedEventsNb, bool supported4CharLabels, ProcessorType processorFile)
        {
            C3dFileHeaderEvent[] events = new C3dFileHeaderEvent[definedEventsNb];
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

        /// <summary>
        /// Parses a byte array into a single <see cref="C3dFileHeaderEvent"/> instance.
        /// </summary>
        /// <param name="binEventTime">The byte array representing the event time.</param>
        /// <param name="binHeaderEventFlag">The byte array representing the event display flag.</param>
        /// <param name="binEventLabel">The byte array representing the event label.</param>
        /// <param name="supported4CharLabels">Indicates whether the C3D file supports 4-character event labels.</param>
        /// <param name="processorFile">The <see cref="ProcessorType"/> used to interpret the byte order of the binary data.</param>
        /// <returns>A <see cref="C3dFileHeaderEvent"/> instance populated with the parsed data.</returns>
        public static C3dFileHeaderEvent EventFromBinaries(
            byte[] binEventTime,
            byte[] binHeaderEventFlag,
            byte[] binEventLabel,
            bool supported4CharLabels,
            ProcessorType processorFile
            )
        {
            C3dFileHeaderEvent headerEvent = new C3dFileHeaderEvent();
            headerEvent.EventTime = C3dBytesConvertor.ToFloat(binEventTime, processorFile);
            headerEvent.DisplayFlag = HeaderEventFlagHelper.FromByte(binHeaderEventFlag[0]);
            // Assuming the label is a fixed length string of 16 bytes
            headerEvent.EventLabel = System.Text.Encoding.ASCII.GetString(binEventLabel, 0, supported4CharLabels?4:2).TrimEnd('\0');
            return headerEvent;
        }

        
    }

    
}