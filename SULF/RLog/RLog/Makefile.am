
all-local: RLog.dll

SRCS =	\
	AssemblyInfo.cs \
	LogComponent.cs \
	LogNode.cs \
	Log.cs \
	Publication.cs

EXTRA_DIST = RLog.dll $(SRCS)

# local and system library references
# Local refs are looked for in ../NAME
LOCAL_REFS = 
# system refs just get a -r:NAME flag for mcs
SYSTEM_REFS = 
# package refs get -pkg:NAME for mcs
PACKAGE_REFS = 

include $(top_srcdir)/Makefile.csharp

#SIGNSTAMP=.RLog.sign.stamp

RLog.dll: $(ALL_DEPS)
	$(CS_DLL) -out:$@ $(MCSFLAGS) $(SRCS)
#	-rm -f $(SIGNSTAMP)

#ShowFuseOptions.exe: ShowFuseOptions.cs Fuse.dll
#	$(MCS) -out:$@ $(MCSFLAGS) -r:Fuse.dll -target:exe ShowFuseOptions.cs

#$(SIGNSTAMP): RLog.dll
#	$(call sign_dll,RLog)
#	-touch $(SIGNSTAMP)

#sign: $(SIGNSTAMP)

#install-data-local: $(DLLTARGET) $(SIGNSTAMP)
install-data-local: $(DLLTARGET)
	$(call install_dll,RLog)

uninstall-local:
	$(call uninstall_dll,RLog)

clean-local:
	-rm -f *.dll


