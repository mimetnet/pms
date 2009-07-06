AC_INIT([pms], [2.0], [matthew@cowarthill.com])
AC_CONFIG_SRCDIR([PMS/Makefile.am])
AM_INIT_AUTOMAKE([foreign tar-pax])
AM_MAINTAINER_MODE

AC_PROG_INSTALL

AC_PATH_PROG(AL, al, no)
AC_PATH_PROG(GMCS, gmcs, no)
AC_PATH_PROG(MONO, mono, no)
AC_PATH_PROG(GACUTIL, gacutil, no)
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
AC_PATH_PROG(MDASSEMBLER, mdassembler, no)
AC_PATH_PROG(MONODOC, monodoc, no)
AC_PATH_PROG(MONODOCER, monodocer, no)
AC_PATH_PROG(MONODOCS2HTML, monodocs2html, no)

if test "x$PKG_CONFIG" = "xno"; then
	AC_MSG_ERROR([You need to install pkg-config])
fi

if test "x$GMCS" = "xno"  ; then
	AC_MSG_ERROR([No csharp compiler found])
fi

if test "x$AL" = "xno"  ; then
	AC_MSG_ERROR([No assembly linker found])
fi

if test "x$GACUTIL" = "xno"  ; then
	AC_MSG_ERROR([No gacutil found])
fi

if test "x$MONO" = "xno"; then
	AC_MSG_ERROR([No mono runtime found])
fi

AM_CONDITIONAL(HAVE_MONODOC, test "x$MONODOC" != "xno")

##############################################################################

test "x$prefix" = "xNONE" && prefix=/usr

##############################################################################

AC_ARG_WITH(
	[log4net],
	[  --with-log4net=<pkg name>   Override default log4net package],
	[],
	[with_log4net=log4net]
)

PKG_CHECK_MODULES(LOG4NET, $with_log4net)

##############################################################################

AC_ARG_ENABLE([debug],
	AC_HELP_STRING([--enable-debug], [Create debug files (.mdb)]),
	enable_debug=${enableval}, enable_debug=yes
)

if test "x$enable_debug" = "xyes"; then
	DEBUG_FLAGS='-debug'
fi

##############################################################################

AC_MSG_CHECKING(for Mono.Data.Sqlite)
if test "`${GACUTIL} -l Mono.Data.Sqlite | tail -1 | awk '{print $5}'`" -gt "0"; then
	AC_MSG_RESULT([yes])
	AM_CONDITIONAL(HAVE_SQLITE, test a = a)
else
	AC_MSG_RESULT([no])
	AM_CONDITIONAL(HAVE_SQLITE, test a = b)
fi

##############################################################################

AC_MSG_CHECKING(for Npgsql)
if test "`${GACUTIL} -l Npgsql | tail -1 | awk '{print $5}'`" -gt "0"; then
	AC_MSG_RESULT([yes])
	AM_CONDITIONAL(HAVE_NPGSQL, test a = a)
else
	AC_MSG_RESULT([no])
	AM_CONDITIONAL(HAVE_NPGSQL, test a = b)
fi

##############################################################################

AC_MSG_CHECKING(for System.Data.SqlClient)
AC_MSG_RESULT([no])
AM_CONDITIONAL(HAVE_MSSQL, test a = b)

##############################################################################

AC_SUBST(AL)
AC_SUBST(GMCS)
AC_SUBST(MONO)
AC_SUBST(GACUTIL)
AC_SUBST(PKG_CONFIG)
AC_SUBST(DEBUG_FLAGS)
AC_SUBST([GACUTIL_FLAGS], ['-root $(DESTDIR)$(prefix)/lib'])
AC_SUBST([MONODOC_SRC_DIR], [`$PKG_CONFIG monodoc --variable=sourcesdir`])
AC_SUBST([MONO_VERSION], [MONO_`$GMCS --version | awk '{print $5}' | sed 's/\./ /g' | awk '{print $1"_"$2}'`])

##############################################################################

if test "dapper" != "`lsb_release -sc`"; then
	AC_SUBST([pmsdir], $libdir/cli/pms-$VERSION)
else
	AC_SUBST([pmsdir], $libdir/mono/2.0)
fi

##############################################################################

AC_OUTPUT([
			  Makefile
			  PMS/Makefile
			  PMS.Data.Pgsql/Makefile
			  PMS.Data.Sqlite/Makefile
			  build/pms-2.0.pc
			  build/Makefile
			  scripts/Makefile
			  policy/Makefile
			  doc/Makefile
			  doc/man/Makefile
])
