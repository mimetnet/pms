SUBDIRS = PMS PMS.Data.Pgsql 

if HAVE_SQLITE
	SUBDIRS += PMS.Data.Sqlite 
endif

SUBDIRS += scripts policy build

if HAVE_MONODOC
	SUBDIRS += doc
endif

EXTRA_DIST = PMS.snk

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g"	\
		 -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
		 -e "s|\@bindir\@|$(bindir)|g"	\
		 -e "s|\@MONO\@|$(MONO)|g" 
