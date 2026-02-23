using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.Supported
{
    public static class ParameterTypeMapper
    {
        private static readonly Dictionary<string, SupportedParameter> Map = new Dictionary<string, SupportedParameter>
        {
            // REQUIRED PARAMETERS
            // POINT
            // Required
            { "POINT:USED", SupportedParameterType.Force },
            { "POINT:SCALE", SupportedParameterType.Force },
            { "POINT:RATE", SupportedParameterType.Force },
            { "POINT:DATA_START", SupportedParameterType.Force },
            { "POINT:FRAMES", SupportedParameterType.Force },
            { "POINT:LABELS", SupportedParameterType.Force },
            { "POINT:DESCRIPTION", SupportedParameterType.Force },
            { "POINT:UNITS", SupportedParameterType.Force },
            // Additional
            { "POINT:LONG_FRAMES", SupportedParameterType.Force },
            { "POINT:LABELS2", SupportedParameterType.Force },
            { "POINT:DESCRIPTIONS2", SupportedParameterType.Force },
            // Application
            { "POINT:X_SCREEN", SupportedParameterType.Force },
            { "POINT:Y_SCREEN", SupportedParameterType.Force },

            // ANALOG
            // Required
            { "ANALOG:USED", SupportedParameterType.Force },
            { "ANALOG:LABELS", SupportedParameterType.Force },
            { "ANALOG:DESCRIPTIONS", SupportedParameterType.Force },
            { "ANALOG:GEN_SCALE", SupportedParameterType.Force },
            { "ANALOG:OFFSET", SupportedParameterType.Force },
            { "ANALOG:UNITS", SupportedParameterType.Force },
            { "ANALOG:SCALE", SupportedParameterType.Force },
            { "ANALOG:RATE", SupportedParameterType.Force },
            { "ANALOG:FORMAT", SupportedParameterType.Force },
            { "ANALOG:BITS", SupportedParameterType.Force },
            // Additional
            { "ANALOG:LABELS2", SupportedParameterType.Force },
            { "ANALOG:DESCRIPTIONS2", SupportedParameterType.Force },
            { "ANALOG:SCALE2", SupportedParameterType.Force },
            { "ANALOG:OFFSET2", SupportedParameterType.Force },
            { "ANALOG:UNITS2", SupportedParameterType.Force },
            // Application
            { "ANALOG:GAIN", SupportedParameterType.Force },

            //FORCE_PLATEFORM
            // Required
            { "FORCE_PLATEFORM:USED", SupportedParameterType.Force },
            { "FORCE_PLATEFORM:TYPE", SupportedParameterType.Force },
            { "FORCE_PLATEFORM:ZERO", SupportedParameterType.Force },
            { "FORCE_PLATEFORM:CORNERS", SupportedParameterType.Force },
            { "FORCE_PLATEFORM:ORIGIN", SupportedParameterType.Force },
            { "FORCE_PLATEFORM:CHANNEL", SupportedParameterType.Force },
            // Additional
            { "FORCE_PLATEFORM:CAL_MATRIX", SupportedParameterType.Force },

            // ADDITIONAL PARAMETERS
            // TRIAL
            // Additional
            { "TRIAL:ACTUAL_START_FIELD", SupportedParameterType.Force },
            { "TRIAL:ACTUAL_END_FIELD", SupportedParameterType.Force },
            { "TRIAL:FRAME_CALCULATION", SupportedParameterType.Force }, // check
            { "TRIAL:CAMERA_RATE", SupportedParameterType.Force },

            // EVENT
            // Additional
            { "EVENT:USED", SupportedParameterType.Force },
            { "EVENT:CONTEXTS", SupportedParameterType.Force },
            { "EVENT:LABELS", SupportedParameterType.Force },
            { "EVENT:DESCRIPTIONS", SupportedParameterType.Force },
            { "EVENT:TIMES", SupportedParameterType.Force },
            { "EVENT:SUBJECTS", SupportedParameterType.Force },
            { "EVENT:ICON_IDS", SupportedParameterType.Force },
            { "EVENT:GENERIC_FLAGS", SupportedParameterType.Force },

            // EVENT_CONTEXT
            // Additional
            { "EVENT_CONTEXT:USED", SupportedParameterType.Force },
            { "EVENT_CONTEXT:ICON_IDS", SupportedParameterType.Force },
            { "EVENT_CONTEXT:LABELS", SupportedParameterType.Force },
            { "EVENT_CONTEXT:COLOURS", SupportedParameterType.Force },

            // APPLICATION PARAMETERS
            // ANALYSIS
            // Application
            { "ANALYSIS:", SupportedParameterType.Force }, // check

            // MANUFACTURER
            // Application
            { "MANUFACTURER:COMPANY", SupportedParameterType.Force },
            { "MANUFACTURER:SOFTWARE", SupportedParameterType.Force },
            { "MANUFACTURER:VERSION", SupportedParameterType.Force },
            { "MANUFACTURER:EDITED", SupportedParameterType.Force },

            // SEG
            // Application
            { "SEG:MARKER_DIAMETER", SupportedParameterType.Force },
            { "SEG:DATA_LIMITS", SupportedParameterType.Force },
            { "SEG:ACC_FACTOR", SupportedParameterType.Force },
            { "SEG:NOISE_FACTOR", SupportedParameterType.Force },
            { "SEG:RESIDUAL_ERROR_FACTOR", SupportedParameterType.Force },
            { "SEG:INTERSECTION_LIMIT", SupportedParameterType.Force },

            // SUBJECTS
            // Application
            { "SUBJECTS:", SupportedParameterType.Force }, // check

        };
        public static SupportedParameter FromString(string groupName, string parameterName)
        {
               
        }
    }
}
