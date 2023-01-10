
all: all-am test.exe

EXTRA_DIST = test.cs

# local and system library references
# Local refs are looked for in ../NAME
LOCAL_REFS = RLog
# system refs just get a -r:NAME flag for mcs
SYSTEM_REFS = 
# package refs get -pkg:NAME for mcs
PACKAGE_REFS = 

include $(top_srcdir)/Makefile.csharp

test.exe: test.cs ../RLog/RLog.dll
	$(CS_EXE) $(MCSFLAGS) -out:$@ test.cs

clean:
	-rm *.exe

distclean: clean

.PHONY: all clean install uninstall distclean


