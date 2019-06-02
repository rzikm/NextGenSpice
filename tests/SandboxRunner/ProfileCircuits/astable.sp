ASTABLE CKT - A SIMPLE ASTABLE MULTIVIBRATOR

.model qstd npn(is=1.0e-16 br=50 br=0.1 rb=50 rc=10 tf=0.12ns tr=5ns
+   cje=0.4pf vje=0.8 mje=0.4 cje=0.5pf vjc=0.8 mjc=0.333 cjs=1pf vaf=50)

vin 5 0 pulse(0 5 0 1us 1us 100us 100us)
vcc 6 0 5
rc1 6 1 1k
rc2 6 2 1k
rb1 6 3 30k
rb2 5 4 30k
c1 1 4 150pf
c2 2 3 150pf
q1 1 3 0 qstd
q2 2 4 0 qstd

*.tran 0.1us 10us
*.print tran v(5) v(1) v(2) v(3) v(4)
*.opt acct 
.end