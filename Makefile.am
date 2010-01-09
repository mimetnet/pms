SUBDIRS = PMS

if HAVE_MSSQL
SUBDIRS += PMS.Driver.MSSQL
endif

if HAVE_NPGSQL
SUBDIRS += PMS.Driver.Pgsql
endif

if HAVE_SQLITE
SUBDIRS += PMS.Driver.Sqlite
endif

SUBDIRS += scripts policy build doc

EXTRA_DIST = PMS.snk certs/PMS.snk certs/PMS.pfx

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g" \
		 -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
		 -e "s|\@bindir\@|$(bindir)|g" \
		 -e "s|\@MONO\@|$(MONO)|g"
