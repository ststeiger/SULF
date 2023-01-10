
all-local: hellofs.exe

LOCAL_REFS = FuseWrapper Fuse Sulf
SYSTEM_REFS = Mono.Posix
PACKAGE_REFS = #rlog-sharp

include $(top_srcdir)/Makefile.csharp

LIBPATH = LD_LIBRARY_PATH=../FuseWrapper/.libs
MONOPATH = MONO_PATH=../RLog/RLog:../Sulf:../Fuse:../FuseWrapper

SRCS = HelloFS.cs

EXTRA_DIST = $(SRCS)

CSFLAGS += $(RLOGSHARP_LIBS)

hellofs.exe: $(SRCS) $(ALL_DEPS)
	$(CS_EXE) -out:$@ $(CSFLAGS) $(SRCS)

# sets appropriate environment variables in order to pick up local libraries..
run: hellofs.exe
	-echo "Creating /tmp/hellofs, and mounting HelloFS example."
	-mkdir /tmp/hellofs
	-echo "Use 'fusermount -u /tmp/hellofs' to unmount hellofs when done."
	$(LIBPATH) $(MONOPATH) mono --debug ./hellofs.exe /tmp/hellofs


#install-data-local: hellofs.exe
#	$(INSTALL) -m 0644 hellofs.exe $(libdir)

#uninstall-local:
#	rm -f $(libdir)/hellofs.exe

clean-local:
	-rm *.dll
	-rm *.exe


