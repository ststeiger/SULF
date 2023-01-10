dnl Process this file with autoconf to produce a configure script.

AC_INIT(rlog.lsm)

dnl This ksh/zsh feature conflicts with `cd blah ; pwd`
unset CDPATH

dnl Checking host/target/build systems, for make, install etc.
AC_CANONICAL_SYSTEM
dnl Perform program name transformation
AC_ARG_PROGRAM

dnl XXX XXX NOTE: should be x.y format only! (not x.y.z), due to use in
dnl RLog/AssemblyInfo.cs
AM_INIT_AUTOMAKE(RLog, 0.3)

unset CDPATH
dnl make /usr/local the default for the installation
AC_PREFIX_DEFAULT(/usr/local)

if test "x$prefix" = "xNONE"; then
  prefix=$ac_default_prefix
  ac_configure_args="$ac_configure_args --prefix $prefix"
fi

dnl without this order in this file, automake will be confused!
dnl
AM_CONFIG_HEADER(config.h)

dnl look for C# compiler
#AC_PATH_PROG(MCS, mcs)
PKG_CHECK_MODULES(MONO, mono > 1.0, [], 
    AC_MSG_WARN([No pkg-config found for mono.. will try anyway])
    have_mono="no")

AC_MSG_CHECKING([For gacutil package destination])
if test "$have_mono" = "no" ; then
    MONO_LIBPATH="/usr/lib"
    AC_MSG_RESULT("assuming $MONO_LIBPATH");
else
    MONO_LIBPATH=`$PKG_CONFIG --variable=libdir mono`
    AC_MSG_RESULT($MONO_LIBPATH)
fi
AC_SUBST(MONO_LIBPATH)

# allow documentation build to be manually disabled
AC_ARG_ENABLE(docs,
    [	--disable-docs	    disable documentation build],
    build_docs=$enableval, build_docs="yes")

# check for tools necessary to build documentation
AC_PATH_PROG(DOXYGEN, doxygen, [no])
AC_PATH_PROG(LATEX, latex, [no])
AC_PATH_PROG(PDFLATEX, pdflatex, [no])

AM_CONDITIONAL(BUILD_DOCS, test "x$build_docs" = "xyes" \
		             -a "x$DOXYGEN" != "xno" \
			     -a "x$LATEX" != "xno" \
			     -a "x$PDFLATEX" != "xno" )


AC_OUTPUT(  [RLog/AssemblyInfo.cs] \
	    [rlog-sharp.pc] \
	    [Makefile] \
	    [RLog/Makefile] \
	    [tests/Makefile])

