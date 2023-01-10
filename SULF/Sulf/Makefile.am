TARGET = Sulf.dll

all-local: $(TARGET) 

SRCS =	AssemblyInfo.cs \
	GenericNode.cs \
	GenericFileNode.cs \
	StringNode.cs \
	MapFS.cs \
	Mounter.cs

EXTRA_DIST = Sulf.dll $(SRCS)

LOCAL_REFS   = Fuse FuseWrapper
SYSTEM_REFS  = 
PACKAGE_REFS = #rlog-sharp

include $(top_srcdir)/Makefile.csharp

CSFLAGS += @RLOGSHARP_LIBS@

$(TARGET): $(REF_DEPS) $(SRCS)
	$(CS_DLL) -out:$@ $(CSFLAGS) $(SRCS)

install-data-local: $(TARGET)
	$(call install_dll, Fuse)

uninstall-local:
	$(call uninstall_dll, Fuse)

clean-local:
	-rm *.dll


