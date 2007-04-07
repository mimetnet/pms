SUBDIRS = PMS PMS.Config build debian

EXTRA_DIST = PMS.snk

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g"	\
		 -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
		 -e "s|\@bindir\@|$(bindir)|g"	\
		 -e "s|\@MONO\@|$(MONO)|g" 
