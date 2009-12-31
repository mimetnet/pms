SUBDIRS = PMS scripts policy build doc

if HAVE_MSSQL
SUBDIRS += PMS.Data.MSSQL
endif

if HAVE_NPGSQL
SUBDIRS += PMS.Data.Pgsql
endif

if HAVE_SQLITE
SUBDIRS += PMS.Data.Sqlite
endif

EXTRA_DIST = PMS.snk certs/PMS.snk certs/PMS.pfx

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g"	\
		 -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
		 -e "s|\@bindir\@|$(bindir)|g"	\
		 -e "s|\@MONO\@|$(MONO)|g" 
