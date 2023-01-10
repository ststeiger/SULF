include $(top_srcdir)/Makefile.csharp

#USER_INCLUDES = @MONO_CFLAGS@ @RLOG_CFLAGS@
#USER_LDFLAGS = @MONO_LIBS@ @RLOG_LIBS@

AM_CFLAGS = -DRLOG_COMPONENT="FuseWrapper" $(USER_INCLUDES)

# pre-build sources for distribution
EXTRA_DIST = fuse_wrap.c fuse.swig *.i *.cs FuseWrapper.dll \
	     autotypemap extract-other typedef-replace 

lib_LTLIBRARIES = libFuseWrapper.la

FUSEHEADER = fuse-2.3.h

libFuseWrapper_la_SOURCES = fuse_wrap.c 
noinst_HEADERS = ${FUSEHEADER} FWBuffer.h

# CURRENT : REVISION : AGE
# +1 : ? : +1  => new interface that does not break old ones
# +1 : 0 : 0   => new interface that breaks old one
#  ? : 0 : 0   => no new interfaces, but breaks apps
#  ? : +1: ?   => internal changes, nothing breaks, but may work better..
#
libFuseWrapper_la_LDFLAGS = -version-info 1:0:0 $(USER_LDFLAGS)

DLLTARGET = FuseWrapper.dll

# fuse-2.3.h is broken down into three files:
# fuse-types.i   : processed with typedef-replace
# fuse-typemap.i : processed with autotypemap
# fuse-extract.i : processed with extract-other
fuse-types.i: ${FUSEHEADER} typedef-replace
	gcc -E ${FUSEHEADER} | ./typedef-replace > $@

fuse-typemap.i: ${FUSEHEADER} autotypemap
	./autotypemap < ${FUSEHEADER} > $@

fuse-extract.i: ${FUSEHEADER} extract-other
	./extract-other < ${FUSEHEADER} > $@

GENERATED = fuse-types.i fuse-typemap.i fuse-extract.i

if HAVE_SWIG
fuse_wrap.c: fuse.swig ${FUSEHEADER} ${GENERATED} FWBuffer.h
	swig -csharp fuse.swig

FuseWrapper.cs: fuse.swig ${FUSEHEADER} ${GENERATED} FWBuffer.h
	swig -runtime -csharp fuse.swig
endif

all-local: $(DLLTARGET)

$(DLLTARGET): FuseWrapper.cs AssemblyInfo.cs
	$(CS_DLL) -out:$@ $(MCSFLAGS) *.cs

install-data-local: $(DLLTARGET)
	$(INSTALL) -m 0644 $(DLLTARGET) $(libdir)

uninstall-local:
	rm -f $(libdir)/$(DLLTARGET)

clean-local: 
	-rm -f *.dll
	-rm -f fuse_*.cs
	-rm -f FuseWrapper*.cs
	-rm -f FWBuffer.cs
	-rm -f fuse_wrap.c
	-rm -f SWIGTYPE_*.cs
	-rm -f FileBuffer.cs
	-rm -f *.i


