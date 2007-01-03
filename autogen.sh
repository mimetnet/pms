#!/bin/sh

srcdir=`dirname $0`

test -z "$srcdir" && srcdir=.

## check for autoconf #######################################
echo -n "Checking for autoconf ..."
(autoconf --version | head -1) || {
  echo
  echo "**Error**: You must have \`autoconf' installed to compile."
  exit 1
}

## check for libtool
echo -n "Checking for libtool..."
if [ -z "$LIBTOOL" ]; then
  LIBTOOL=`which glibtool 2>/dev/null` 
  LIBTOOLIZE=`which glibtoolize 2>/dev/null`
  if [ ! -x "$LIBTOOL" ]; then
    LIBTOOL=`which libtool`
    LIBTOOLIZE=`which libtoolize 2>/dev/null`
  fi

  if [ "" = "$LIBTOOL" ]; then
    echo
    echo "**Error**: You must have libtool installed"
    exit 1
  fi
  $LIBTOOL --version | head -1 
fi

$LIBTOOLIZE --automake 


## check for automake
echo -n "Checking for automake ..."
(automake --version | head -1) || {
  echo
  echo "**Error**: You must have \`automake' installed to compile."
  exit 1
}

## if no automake, don't bother testing for aclocal
echo -n "Checking for aclocal ..."
(aclocal --version | head -1) || {
  echo
  echo "**Error**: Missing \`aclocal'.  The version of \`automake'"
  echo "installed doesn't appear recent enough."
  exit 1
}


## run commands ##
echo "Running aclocal $ACLOCAL_FLAGS ..."
aclocal $ACLOCAL_FLAGS || {
  echo
  echo "**Error**: aclocal failed. This may mean that you have not"
  echo "installed all of the packages you need, or you may need to"
  echo "set ACLOCAL_FLAGS to include \"-I \$prefix/share/aclocal\""
  echo "for the prefix where you installed the packages whose"
  echo "macros were not found"
  exit 1
}

if grep "^AM_CONFIG_HEADER" configure.in >/dev/null; then
  echo "Running autoheader ..."
  autoheader || { echo "**Error**: autoheader failed."; exit 1; }
fi

echo "Running automake --gnu $am_opt ..."
automake --add-missing --gnu $am_opt ||
  { echo "**Error**: automake failed."; exit 1; }
echo "Running autoconf ..."
autoconf || { echo "**Error**: autoconf failed."; exit 1; }

echo
$srcdir/configure
echo
echo Then run \`make\' to compile $PKG_NAME
echo

exit 0
