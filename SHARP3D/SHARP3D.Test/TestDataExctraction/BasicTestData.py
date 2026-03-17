import glob
import os
from ezc3d import c3d
import json
import sys
import simplejson

# Just to have arguments for debug:
# ../SampleFiles/Sample03/

c3d_files = glob.glob(f"{sys.argv[1]}/*.c3d")


for path in c3d_files:
    filename = os.path.splitext(os.path.basename(path))[0]
    foldername = os.path.splitext(os.path.dirname(path))[0]
    try:
        c = c3d(path, ignore_bad_formatting=True)
        
        groups_parameter = [group for group in list(c['parameters'].keys()) if group not in {"__METADATA__", "", None}]
        

        parameters = list()

        for group in groups_parameter:
            if group in {"__METADATA__", "", None}:
                continue
            # parameters.append([parameter for parameter in c['parameters'][group] if parameter not in {"__METADATA__","BITS","FORMAT"}])
            temp = list()
            for parameter in c['parameters'][group]:
                if parameter not in {"__METADATA__", ""}:
                    if (parameter == "FORMAT") and len(c["parameters"]["ANALOG"]["FORMAT"]["value"]) == 0:
                        continue
                    if (parameter == "BITS") and len(c["parameters"]["ANALOG"]["BITS"]["value"]) == 0:
                        continue
                    if (parameter == "CAL_MATRIX") and (c["parameters"]["FORCE_PLATFORM"]["CAL_MATRIX"]["value"]).size == 0:
                       continue
                    temp.append(parameter) 
            parameters.append(temp)
        point_first_0 = c['data']['points'][:3,0,0].tolist()
        point_last_0 = c['data']['points'][:3,0,-1].tolist()

        analog_first_0 = c['data']['analogs'][0,0,0]
        analog_last_0 = c['data']['analogs'][0,0,-1]

        point_frames = c['data']['points'].shape[-1]
        analog_frames = c['data']['analogs'].shape[-1]

        data ={
            "groups_parameter": groups_parameter,
            "parameters": parameters,
            "point_first_0": point_first_0,
            "point_last_0": point_last_0,
            "analog_first_0": analog_first_0,
            "analog_last_0": analog_last_0,
            "point_frames": point_frames,
            "analog_frames": analog_frames,
        }


        with open(f"{foldername}/{filename}.json", "w") as f:
            json_string = simplejson.dumps(data, indent=4, ignore_nan=True)
            f.write(json_string)
            
    except Exception as e:
        print(f"{e}\n")
        with open("./basic_test_exceptions.txt", "a") as file:
            file.write(f"{foldername}/{filename}.c3d: {e}\n")
