param([string]$infile)
if ($infile.LastIndexOf('.') -ne -1)
{
    $base = $infile.Substring(0, $infile.LastIndexOf('.'));
}
$outfile = $base + ".out"

# run app for output
$output = dotnet .\NextGenSpice.dll "$infile"

$success = $LASTEXITCODE -eq 0

if ($success)
{    
    #skip the .TRAN header
    $output = echo $output | Select-Object -Skip 1
}

echo $output | Out-File -Encoding ascii -FilePath "$outfile";


if (-not $success)
{
    # exit
    echo "An error occured, check '$outfile' for details";
    exit 1;
}

# get number of data columns
$count = (cat -first 1 "$outfile").Split(' ').Count;
$plots = $count
if ($count -eq 2) # only one variable printed
{
    $plots = 1;
}

# run gnuplot to get .svg file
"set terminal svg size 900, $plots*250
set output '$base.svg'
set key autotitle columnhead
set lmargin 8
set multiplot layout $plots, 1 ;
set grid ytics lt 0 lw 1 lc rgb '#bbbbbb'
set grid xtics lt 0 lw 1 lc rgb '#bbbbbb'
plot for [i=2:$count] '$outfile' using 1:i with lines
do for [i=2:$count] {
    plot '$outfile' using 1:i with lines
}
" | gnuplot
# open output .svg file
iex "& `"$base.svg`""
