export TEXINPUTS=../tex//:

all: thesis.pdf

# LaTeX must be run multiple times to get references right
thesis.pdf: thesis.tex $(wildcard *.tex) bibliography.bib thesis.xmpdata
	pdflatex -interaction=nonstopmode -shell-escape $<
	bibtex thesis
	pdflatex -interaction=nonstopmode -shell-escape $<
	pdflatex -interaction=nonstopmode -shell-escape $<

clean:
	rm -f *.log *.dvi *.aux *.toc *.lof *.lot *.out *.bbl *.blg *.xmpi
	rm -f thesis.pdf
