*** double double test circuit from https://www.circuitlab.com/blog/2013/07/22/double-double-please-when-64-bit-floating-point-isnt-enough/ ***

V1 IN 0 SIN(0 5 100 0 0 0)
D1 IN A 1N4148
R1 A B 1E-6
D2 0 B 1N4148

.MODEL 1N4148 D(IS=2.52N RS=.568 N=1.752 CJO=4P M=.4 TT=20N VJ=20 BV=75)
*.PRINT TRAN V(IN) I(D1)

*.TRAN 100US 10M
