EXTRA_DIST = pms.zip pms.tree pms.source
CLEANFILES = pms.zip pms.tree pms.stamp

all: pms.stamp

pms.stamp:
	touch $@
	mdassembler --ecma api -o pms

api: ../PMS/PMS.dll
	monodocer -assembly:$< -name:$(PACKAGE) -path:api -pretty

api-html:
	monodocs2html --source api --dest api-html

install-data-local:
	$(install_sh_DATA) -m 755 -d $(DESTDIR)`$(PKG_CONFIG) monodoc --variable=sourcesdir`/
	$(install_sh_DATA) -t $(DESTDIR)`$(PKG_CONFIG) monodoc --variable=sourcesdir`/ $(EXTRA_DIST)

uninstall-local:
	for file in $(EXTRA_DIST); do \
		if test -f $(DESTDIR)`$(PKG_CONFIG) monodoc --variable=sourcesdir`/$$file; then \
			rm $(DESTDIR)`$(PKG_CONFIG) monodoc --variable=sourcesdir`/$$file; \
		fi; \
	done;
