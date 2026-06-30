# FORCE_PLATFORM:CAL_MATRIX

- **Type**: [Additional](../../additional.md)

- **Locked**: False

This parameter seems to be of the form [6,6,USED].

A calibration matrix enables software applications to correct for cross talk between outputs of the force platform. Software applications that use the full calibration matrix to correct for cross talk will typically provide more accurate results when compared to applications that only have access to the major diagonal component.

Below is a typical manufacturer's crosstalk matrix supplied in Newton-meters (Nm). The major diagonal components are highlighted in bold.

|  | $F_x$ | $F_y$ | $F_z$ | $M_z$ | $M_y$ | $M_z$ |
| --- | --- | --- | --- | --- | --- | --- |
| $V_{f_x}$ | **0.3405** | 0.0004 | 0.0005 | 0.0029 | 0.0012 | 0.0100 |
| $V_{f_y}$ | -0.0003 | **0.3395** | 0.0006 | 0.0051 | 0.0043 | -0.0004 |
| $V_{f_z}$ | -0.0011 | 0.0001 | **0.0862** | 0.0011 | -0.0004 | -0.0019 |
| $V_{m_x}$ | 0.0002 | 0.0008 | 0.0003 | **0.7918** | -0.0006 | 0.0065 |
| $V_{m_y}$ | 0.0008 | 0.0000 | -0.0007 | -0.0004 | **0.7884** | 0.0103 |
| $V_{m_z}$ | 0.0027 | 0.0011 | 0.0003 | -0.0015 | 0.0017 | **1.7005** |

*Output of channel i(uV/Vex) $=$ S(I,j) $\times$ mechanical input j(N, Nm).*

Since the CAL_MATRIX parameter will be ignored, even if present, unless the force platform type is a supported [TYPE](../../required/force_platform/force_platform-type.md), its inclusion in a C3D file does not automatically imply that it must be applied to the stored force data. If the force data [TYPE](../../required/force_platform/force_platform-type.md) does not support the CAL_MATRIX then the force plate’s data must be scaled using the [ANALOG:SCALE](../../required/analog/analog-scale.md) factors as described in detail in the chapter entitled “[Calculating SCALE values for force plates](../../required/analog/analog-scale.md#force-plates)”.

Note that most force plate systems include some degree of variable amplification of the signals from the plate. The amount of amplification applied to each force signal must be taken into account when applying the calibration matrix and is an important factor is the calculation of the correct [ANALOG:SCALE](../../required/analog/analog-scale.md) value for each force plate channel.

The calibration matrix for each force platform must be applied to the measured channel outputs to obtain the corrected channel outputs according to the matrix equation:

$\mathbf{CAL\_MATRIX} \cdot \vec{F_{measured}} = \vec{F_{corrected}}$

where the $\vec{F}$ are column vectors. The elements of the calibration matrix will always be stored in column order:

$$
\begin{pmatrix}
C_{11} & C_{12} & C_{13} & C_{14} & C_{15} & C_{16}\\
C_{21} & C_{22} & C_{23} & C_{24} & C_{25} & C_{26}\\
C_{31} & C_{32} & C_{33} & C_{34} & C_{35} & C_{36}\\
C_{41} & C_{42} & C_{43} & C_{44} & C_{45} & C_{46}\\
C_{51} & C_{52} & C_{53} & C_{54} & C_{55} & C_{66}\\
C_{61} & C_{62} & C_{63} & C_{64} & C_{65} & C_{66}\\
\end{pmatrix}
$$

For the first force platform using a 6x6 $\mathbf{CAL\_MATRIX}$:

- $\mathbf{CAL\_MATRIX}_{1,1,1}$ must contain the first element of the matrix.

- $\mathbf{CAL\_MATRIX}_{6,1,1}$ the last element of the first column.

- $\mathbf{CAL\_MATRIX}_{1,2,1}$ must contain the first element of the second column, etc.

The first three rows of the supplied calibration matrix have units of force/Volt (e.g. N/V) and the last three rows have units of moments/Volt (e.g. N•m/V). If the C3D file is using distance units of millimeters then the last three rows of the calibration matrix must have units of N•mm/V. In order to convert from N•m/V to N•mm/V each element in the last three rows must be multiplied by 1000.

|  | $F_x$ | $F_y$ | $F_z$ | $M_z$ | $M_y$ | $M_z$ |
| --- | --- | --- | --- | --- | --- | --- |
| $V_{f_x}$ | 0.3405 | 0.0004 | 0.0005 | 2.900 | 1.200 | 10.00 |
| $V_{f_y}$ | -0.0003 | 0.3395 | 0.0006 | 5.100 | 4.300 | -0.400 |
| $V_{f_z}$ | -0.0011 | 0.0001 | 0.0862 | 1.100 | -0.400 | -1.900 |
| $V_{m_x}$ | 0.0002 | 0.0008 | 0.0003 | 791.8 | -0.600 | 6.500 |
| $V_{m_y}$ | 0.0008 | 0.0000 | -0.0007 | -0.400 | 788.4 | 10.30 |
| $V_{m_z}$ | 0.0027 | 0.0011 | 0.0003 | -1.500 | 1.700 | 1700.5 |

*Output of channel i(uV/Vex) $=$ S(I,j) $\times$ mechanical input j(N, Nm).*

Note that the analog channels associated with force platforms using the $\mathbf{CAL_MATRIX}$ must be scaled in Volts ([see the earlier discussions for full details on calculating the analog scale values for each force platform type](../../required/analog/analog-scale.md#force-plates)). [Sample data files and spreadsheets are available from the C3D web site](https://www.c3d.org/sampledata.html) that implements the CAL_MATRIX parameter calculations for the associated analog channels.

When implementing the CAL_MATRIX parameter it is very important to be aware of the order in which the C3D format stores the elements of the matrix: **the storage sequence is in column order (as in FORTRAN) and not row order (as in C and C++)**. <!--Or anything really... Love the antics-->Also, every C3D file uses a consistent set of units throughout. thus while the force plate manufacturer usually supplies the moment calibration data in terms of N•m/V, the calibration matrix must store the moment data in N•mm/V if the POINT calibration and measurement units are millimeters.

<!--hex dump -->

For example, if we have a 6x6 CAL_MATRIX parameter stored in the C3D file then the first three rows will have units of newtons per Volt and the second three rows will have units of newton•millimeters per Volt (Nm/V * 1000).

If the analog signals from the six force plate sensors are scaled as Volts in the column vector $\vec{V}$: 

$$
\begin{pmatrix}
V_1\\
V_2\\
V_3\\
V_4\\
V_5\\
V_6\\
\end{pmatrix}
$$

Resulting in the corrected forces and moments as the column vector $\vec{W}$:

$$
\begin{pmatrix}
W_1\\
W_2\\
W_3\\
W_4\\
W_5\\
W_6\\
\end{pmatrix}
$$

Then using the standard notation:

$W = C*V$

Note that $W_1$ is computed by:

$W_1 = C_{11}*V_1 +C_{12}*V_2 +C_{13}*V_3 +C_{14}*V_4 +C_{15}*V_5 +C_{16}*V_6$

And that the resulting $W_1$, $W_2$, $W_3$ will have units of newtons, and $W4$, $W5$, $W6$ will have units of newton•millimeters.

The presence of the FORCE_PLATFORM:CAL_MATRIX parameter in a C3D file means that users and researchers retain the ability to determine the quality of the force plate data in a C3D file in any environment instead of trusting that unseen calculations were performed correctly in the past
