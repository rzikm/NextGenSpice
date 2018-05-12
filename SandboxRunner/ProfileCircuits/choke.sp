CHOKE CKT - FULL WAVE CHOKE INPUT

.MODEL DIO D(IS=1E-14 CJO=10PF)

VIN1 1 0 SIN(0 100 50)
VIN2 2 0 SIN(0 -100 50)
D1 1 3 DIO
D2 2 3 DIO
R1 3 0 10K
L1 3 4 5.0
R2 4 0 10K
C1 4 0 2UF

*.TRAN 0.2MS 20MS
*.PRINT TRAN V(1) V(2) V(3) V(4)
.END
