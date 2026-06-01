# MANUFACTURER:EDITED

- **Type**: [Application](../../application.md)

- **Locked**: False

This optional parameter enables a record of changes made to the C3D file to be maintained. Typically, the name of the application editing the C3D file and the date and time of any changes made to the file contents are recorded here.

> This allows any changes made to a C3D file to be documented after the C3D file has been modified in any way.

C3D files are often processed by additional applications which may change the C3D file contents. The EDITED parameter can be created as a character array to record, as individual ASCII or UTF-8 strings, basic information in sequence of each application that modifies the C3D file contents. The first application to edit a C3D file may create this parameter as a single entry array; subsequent edits can then extend the array to create additional entries.

The EDITED parameter should be unlocked, and additional array entries may be added by software applications without changing any prior entries that the current application did not create, to preserve a specific C3D file processing history. An application that creates a series of EDITED entries while processing a C3D file can delete and edit any entries that it has created, while preserving EDITED records created by other applications. Each entry added to the EDITED array can be up to 255 characters in length and should normally preserve any prior entries.