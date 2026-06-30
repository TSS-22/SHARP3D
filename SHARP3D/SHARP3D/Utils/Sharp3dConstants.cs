using SHARP3D.Utils.Enum;

namespace SHARP3D.Utils
{
    /// <summary>
    /// Provides program-wide constants for SHARP3D, including arrays representing signed and unsigned string literals.
    /// </summary>
    internal static class Sharp3dConstants
    {
        /// <summary>
        /// A read-only <see cref="Array"/> of <see cref="char"/> representing the string "SIGNED".
        /// </summary>
        public static readonly Array SignedArrayString;

        /// <summary>
        /// A read-only <see cref="Array"/> of <see cref="char"/> representing the string "UNSIGNED".
        /// </summary>
        public static readonly Array UnsignedArrayString;

        public static readonly Dictionary<ForceplateType, int> ForceplateChannelNumber = new Dictionary<ForceplateType, int>
            {
                { ForceplateType.UNKOWN, 0 },
                { ForceplateType.TYPE_1, 6 },
                { ForceplateType.TYPE_2, 6 },
                { ForceplateType.TYPE_3, 8 },
                { ForceplateType.TYPE_4, 6 },

            };

        // WARNING: 
        // This does not encompass the parameter with format XXX[0-9]*
        public static readonly Dictionary<string, string[]> ParameterToDiscardFromC3dFileToC3d = new Dictionary<string, string[]>
        {
            { "ANALOG", new string[]
                {
                    "BITS",
                    "DESCRIPTIONS",
                    "FORMAT",
                    "GEN_SCALE",
                    "LABELS",
                    "OFFSET",
                    "RATE",
                    "SCALE",
                    "UNITS",
                    "USED",
                }
            },
            { "FORCE_PLATFORM", new string[]
                {
                    "CAL_MATRIX",
                    "CORNERS",
                    "CHANNEL",
                    "ORIGIN",
                    "TYPE",
                    "USED",
                    "ZERO",
                }
            },
            { "POINT", new string[]
                {
                    "DATA_START",
                    "DESCRIPTIONS",
                    "FRAMES",
                    "LABELS",
                    "LONG_FRAMES",
                    "RATE",
                    "SCALE",
                    "UNITS",
                    "USED",
                }
            },
            {
                "TRIAL", new string[]
                {
                    "ACTUAL_END_FIELD",
                    "ACTUAL_START_FIELD",
                }
            }
        };

        /// <summary>
        /// Initializes the <see cref="SignedArrayString"/> and <see cref="UnsignedArrayString"/> arrays.
        /// </summary>
        static Sharp3dConstants()
        {
            // Initialize SignedArrayString
            SignedArrayString = Array.CreateInstance(typeof(char), "SIGNED".Length);
            for (int i = 0; i < "SIGNED".Length; i++)
            {
                SignedArrayString.SetValue("SIGNED"[i], i);
            }

            // Initialize UnsignedArrayString
            UnsignedArrayString = Array.CreateInstance(typeof(char), "UNSIGNED".Length);
            for (int i = 0; i < "UNSIGNED".Length; i++)
            {
                UnsignedArrayString.SetValue("UNSIGNED"[i], i);
            }

        }
    }
}