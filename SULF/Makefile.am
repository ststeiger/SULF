if INTERNAL_RLOG
RLOG_DIR = RLog
RLOG_DIST = RLog/configure.ac RLog/configure \
	    RLog/rlog.snk RLog/rlog-sharp.pc.in
endif

SUBDIRS = $(RLOG_DIR) FuseWrapper Fuse Sulf examples docs 
#others: RawFS ProcFS sulf

EXTRA_DIST = AUTHORS COPYING ChangeLog INSTALL README \
	     sulf.lsm sulf.snk $(RLOG_DIST)

# not a GNU package. You can remove this line, if
# have all needed files, that a GNU package needs
AUTOMAKE_OPTIONS = foreign

documentation:
	cd docs && $(MAKE)

