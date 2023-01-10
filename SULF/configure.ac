dnl Process this file with autoconf to produce a configure script.

AC_INIT(sulf.lsm) dnl a source file from your sub dir

dnl This ksh/zsh feature conflicts with `cd blah ; pwd`
unset CDPATH

dnl Checking host/target/build systems, for make, install etc.
AC_CANONICAL_SYSTEM
dnl Perform program name transformation
AC_ARG_PROGRAM

AM_INIT_AUTOMAKE(sulf, 0.3)

unset CDPATH
dnl make /usr/local the default for the installation
AC_PREFIX_DEFAULT(/usr/local)

if test "x$prefix" = "xNONE"; then
  prefix=$ac_default_prefix
  ac_configure_args="$ac_configure_args --prefix $prefix"
fi

AM_CONFIG_HEADER(config.h)

dnl first check for c/c++ compilers
AC_PROG_CC
AC_PROG_CPP
#AC_LANG_C

dnl setup libtool stuff
AC_ENABLE_SHARED(yes)
AC_ENABLE_STATIC(no)
AM_PROG_LIBTOOL

dnl adjust the flags before running tests, since they may affect the results
CPPFLAGS="$CPPFLAGS -D_FILE_OFFSET_BITS=64"

# check for documentation build flag
AC_ARG_ENABLE(docs,
    [  --disable-docs      disable documentation build],
    build_docs=$enableval, build_docs="yes")

# check for tools necessary to build documentation
AC_PATH_PROG(DOXYGEN, doxygen, [no])
AC_PATH_PROG(LATEX, latex, [no])
AC_PATH_PROG(PDFLATEX, pdflatex, [no])

# check if we have all the tools necessary to build documentation..
AM_CONDITIONAL(BUILD_DOCS, test "x$build_docs" = "xyes" \
		             -a "x$DOXYGEN" != "xno" \
		             -a "x$LATEX" != "xno" \
		             -a "x$PDFLATEX" != "xno" )

AC_PATH_PROG(SWIG, swig)
if test -n "$SWIG"; then
    AC_MSG_CHECKING([for SWIG >= 1.3.22])
    vers=`swig -version 2>&1 | sed -n 's/SWIG Version //p' | awk 'BEGIN { FS = "."; } { printf "%d", ($1 * 1000 + $2) * 1000 + $3;}'`
    if test -n "$vers" && test "$vers" -ge 1003022
    then
	have_swig="yes"
	AC_MSG_RESULT([yes])
    else
	have_swig="no"
	AC_MSG_RESULT([no])
	AC_MSG_WARN([SWIG version 1.3.22 or later required to build swig files])
    fi
else
    AC_MSG_WARN([SWIG not found, pre-built interface files will be used])
    have_swig="no"
fi

AM_CONDITIONAL(HAVE_SWIG, test "x$have_swig" = "xyes")

# look for C# compiler
AC_PATH_PROG(MCS, mcs)
#PKG_CHECK_MODULES(MONO, mono > 1.0)
if test -n "$MCS"; then
    CSCC="mcs"
else
    AC_MSG_WARN([C-Sharp compiler not found, assuming Mono's mcs])
    CSCC="mcs"
fi
AC_SUBST(CSCC)

#PKG_CHECK_MODULES(RLOG, librlog >= 1.3)
if test -d RLog; then
    # use local RLog
    RLOGSHARP_CFLAGS=""
    RLOGSHARP_LIBS="-L \${top_srcdir}/RLog/RLog -r:\${top_srcdir}/RLog/RLog/RLog.dll"
    AC_MSG_NOTICE([Using internal RLog])
    build_internal_rlog="yes"
    AC_CONFIG_SUBDIRS([RLog])
else
    PKG_CHECK_MODULES(RLOGSHARP, rlog-sharp >= 0.3)
fi
AM_CONDITIONAL(INTERNAL_RLOG, test "x$build_internal_rlog" = "xyes")

AC_CHECK_MEMBERS([struct stat.st_atim])

all_libraries="$all_libraries $USER_LDFLAGS"
all_includes="$all_includes $USER_INCLUDES"
AC_SUBST(all_includes)
AC_SUBST(all_libraries)

AC_SUBST(AUTODIRS)

AC_CONFIG_FILES( [Makefile] \
    [Makefile.csharp] \
    [FuseWrapper/Makefile] \
    [FuseWrapper/AssemblyInfo.cs] \
    [Fuse/Makefile] \
    [Fuse/AssemblyInfo.cs] \
    [Sulf/Makefile] \
    [Sulf/AssemblyInfo.cs] \
    [examples/Makefile] \
    [docs/Makefile])

dnl    [ProcFS/Makefile] \
dnl    [RawFS/Makefile] \
dnl    [librawfs/Makefile] \
dnl    [sulf-exe/Makefile])

AC_OUTPUT

