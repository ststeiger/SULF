TARGET = Fuse.dll
SIGNSTAMP = .Fuse.sign.stamp

all-local: $(TARGET) ShowFuseOptions.exe FinishedBuild

SRCS =	AssemblyInfo.cs \
	Channel.cs \
	DirNode.cs \
	ErrorCode.cs \
	FileNode.cs \
	FileSystem.cs \
	LookupTransaction.cs \
	OpenTransaction.cs \
	Node.cs \
	NodeMap.cs \
	Operation.cs \
	Reactor.cs \
	StaleNodeException.cs \
	Stat.cs \
	StatFS.cs \
	StringArg.cs \
	Transactional.cs \
	XAttrNode.cs

EXTRA_DIST = Fuse.dll $(SRCS) ShowFuseOptions.cs

LOCAL_REFS   = FuseWrapper
SYSTEM_REFS  = 
PACKAGE_REFS = #rlog-sharp

include $(top_srcdir)/Makefile.csharp

CSFLAGS += @RLOGSHARP_LIBS@

$(TARGET): $(REF_DEPS) $(SRCS)
	$(CS_DLL) -out:$@ $(CSFLAGS) $(SRCS)
	-rm -f $(SIGNSTAMP)

ShowFuseOptions.exe: $(REF_DEPS) ShowFuseOptions.cs Fuse.dll
	$(CS_EXE) -out:$@ $(CSFLAGS) -r:Fuse.dll ShowFuseOptions.cs

$(SIGNSTAMP): Fuse.dll
	$(call sign_dll, Fuse)

install-data-local: $(TARGET) $(SIGNSTAMP)
	$(call install_dll, Fuse)

uninstall-local:
	$(call uninstall_dll, Fuse)

clean-local:
	-rm -f $(SIGNSTAMP)
	-rm *.dll

FinishedBuild:
	@echo ""
	@echo "Build Finished."
	@echo "To Run programs built in this directory, you will need to set"
	@echo "MONO_PATH to pick up ../RLog/RLog .  Eg:"
	@echo "MONO_PATH=../RLog/RLog ./ShowFuseOptions.exe"

