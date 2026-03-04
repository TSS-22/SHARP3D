import glob
import os
from ezc3d import c3d
import json
import sys


c3d_files = glob.glob(f"{sys.argv[1]}/*.c3d")


for path in c3d_files:
    filename = os.path.splitext(os.path.basename(path))[0]
    foldername = os.path.splitext(os.path.dirname(path))[0]
    try:
        c = c3d(path)
        
        groups_parameter = list(c['parameters'].keys())

        parameters = list()

        for group in groups_parameter:
            if group == "__METADATA__":
                continue
            parameters.append([parameter for parameter in c['parameters'][group] if parameter not in {"__METADATA__","BITS","FORMAT"}])

        point_first_0 = c['data']['points'][:3,0,0].tolist()
        point_last_0 = c['data']['points'][:3,0,-1].tolist()

        analog_first_0 = c['data']['analogs'][0,0,0]
        analog_last_0 = c['data']['analogs'][0,0,-1]

        point_frames = c['data']['points'].shape[-1]
        analog_frames = c['data']['analogs'].shape[-1]

        camera_mask_first_0 = c["data"]["meta_points"]["camera_masks"][:,0,0]
        camera_mask_last_0 = c["data"]["meta_points"]["camera_masks"][:,0,-1]

        data ={
            "groups_parameter": groups_parameter,
            "parameters": parameters,
            "point_first_0": point_first_0,
            "point_last_0": point_last_0,
            "analog_first_0": analog_first_0,
            "analog_last_0": analog_last_0,
            "point_frames": point_frames,
            "analog_frames": analog_frames,
            "camera_mask_first_0": camera_mask_first_0,
            "camera_mask_last_0": camera_mask_last_0
        }


        with open(f"{foldername}/{filename}.json", "w") as f:
            json.dump(data, f, indent=4)
            
    except Exception as e:
        with open("./exceptions.txt", "a") as file:
            file.write(f"{foldername}/{filename}.c3d: {e}\n")
