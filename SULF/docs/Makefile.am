
SOURCES = index.dox fusewrapper.dox

docdir = $(datadir)/doc
pkgdocdir = $(docdir)/$(PACKAGE)
#pkghtmldir = $(pkgdocdir)/html


# have both html and html/index.html to force build of index.html if necessary
EXTRA_DIST = Doxyfile latex/refman.pdf html html/index.html \
	     *.dox

pkgdoc_DATA = \
    latex/refman.pdf

#pkghtml_DATA = \
#    html/*

if BUILD_DOCS
latex/refman.pdf: html/index.html
	-cd latex && make

html/index.html: Doxyfile $(SOURCES)
	-rm -rf html
	-cd .. && @DOXYGEN@ docs/Doxyfile
else
latex/refman.pdf:
html/index.html:
endif

push-online: html/index.html
	-cd html && rsync -r --stats . arg0.net:public_html/sulf

clean-local:
	-rm -rf html
	-rm -rf latex

all-local: html/index.html

install-data-local: all-local
	-echo installing to ${DESTDIR}${pkgdocdir}/html
	/bin/sh ../admin/mkinstalldirs ${DESTDIR}${pkgdocdir}/html
	install html/* ${DESTDIR}${pkgdocdir}/html


