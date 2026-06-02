# SEG

> Although not required in a C3D file, the SEG group contains useful information about the environment used during the data collection.

The SEG parameter group is common in older C3D file to provide the user with information about the data processing used when the raw data points were tracked and processed. It is also used by other 3D photogrammetry applications and contains application specific values. A full description of the parameters normally contained in this group is available in the original [ADTECH Motion Analysis Software System (AMASS) reference manual](https://www.wiki.has-motion.com/doku.php?id=other:amass:amass2_documentation)<sup id="fnref1"><a href="#fn1">1</a></sup>.

The presence of SEG parameters in a C3D file is optional and normally only serves to provide information that is specific to the application that initially created the C3D file. The information stored in the SEG group is useful since it documents the data collection environment when there is a need to resolve any 3D data collection and tracking issues in the resulting C3D files.

<sup id="fn1">1</sup>. We are not entirely sure this is the right manual, and where to find the information in that manual. If you ever find it, please do let us know. Good luck. [↩] (#fnref1)