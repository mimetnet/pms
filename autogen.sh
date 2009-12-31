#! /bin/sh

PROJECT=PMS
CONFIGURE=configure.ac
LSB_RELEASE=$(lsb_release -sc)
LSB_ID=$(lsb_release -si)

: ${AUTOCONF=autoconf}
: ${AUTOHEADER=autoheader}
: ${AUTOMAKE=automake}
: ${LIBTOOLIZE=libtoolize}
: ${ACLOCAL=aclocal}
: ${LIBTOOL=libtool}

debfolder()
{
	if [ ! -d debian ]; then
		if [ -d debian.${LSB_RELEASE} ]; then
			ln -s debian.${LSB_RELEASE}/ debian
		else
			echo "autogen.sh (debfolder): LSB_RELEASE not package-able: '${LSB_RELEASE}'" >&2
			echo >&2
		fi
	fi
}

case "${LSB_ID}" in
	Debian)
		debfolder;;
	Ubuntu)
		debfolder;;
	*)
		echo "autogen.sh called by unkown LSB_ID: '${LSB_ID}'" >&2
		exit 1
		;;
esac

srcdir=`dirname $0`
test -z "$srcdir" && srcdir=.

ORIGDIR=`pwd`
cd $srcdir
aclocalinclude="-I . $ACLOCAL_FLAGS"

DIE=0

($AUTOCONF --version) < /dev/null > /dev/null 2>&1 || {
        echo
        echo "You must have autoconf installed to compile $PROJECT."
        echo "Download the appropriate package for your distribution,"
        echo "or get the source tarball at ftp://ftp.gnu.org/pub/gnu/"
        DIE=1
}

($AUTOMAKE --version) < /dev/null > /dev/null 2>&1 || {
        echo
        echo "You must have automake installed to compile $PROJECT."
        echo "Get ftp://sourceware.cygnus.com/pub/automake/automake-1.4.tar.gz"
        echo "(or a newer version if it is available)"
        DIE=1
}

(grep "^AM_PROG_LIBTOOL" $CONFIGURE >/dev/null) && {
  ($LIBTOOL --version) < /dev/null > /dev/null 2>&1 || {
    echo
    echo "**Error**: You must have \`libtool' installed to compile $PROJECT."
    echo "Get ftp://ftp.gnu.org/pub/gnu/libtool-1.2d.tar.gz"
    echo "(or a newer version if it is available)"
    DIE=1
  }
}

if test "$DIE" -eq 1; then
        exit 1
fi

if test -z "$*"; then
        echo "I am going to run ./configure with no arguments - if you wish "
        echo "to pass any to it, please specify them on the $0 command line."
		  echo
fi

case $CC in
*xlc | *xlc\ * | *lcc | *lcc\ *) am_opt=--include-deps;;
esac

(grep "^AM_PROG_LIBTOOL" $CONFIGURE >/dev/null) && {
    echo "Running $LIBTOOLIZE ..."
    $LIBTOOLIZE --force --copy
}

echo "Running $ACLOCAL $aclocalinclude ..."
$ACLOCAL $aclocalinclude

echo "Running $AUTOMAKE --copy --gnu $am_opt ..."
$AUTOMAKE --add-missing --copy --gnu $am_opt

echo "Running $AUTOCONF ..."
$AUTOCONF

echo Running $srcdir/configure "$@" ...
$srcdir/configure --enable-maintainer-mode "$@" \

