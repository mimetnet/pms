EXTRA_DIST = pms2.zip pms2.tree pms2.source
CLEANFILES = pms2.zip pms2.tree pms2.stamp

if HAVE_MONODOC
all: pms2.stamp

pms2.stamp: api/index.xml
	$(MDASSEMBLER) --ecma api -o pms2
	touch $@

api/index.xml: $(top_srcdir)/PMS/PMS.dll
	$(MONODOCER) -assembly:$< -name:$(PACKAGE) -path:api -pretty

api-html:
	$(MONODOCS2HTML) --source api --dest api-html

install-data-local:
	$(INSTALL) -m 755 -d $(DESTDIR)$(MONODOC_SRC_DIR)/
	$(INSTALL_DATA) -t $(DESTDIR)$(MONODOC_SRC_DIR)/ $(EXTRA_DIST)

uninstall-local:
	for file in $(EXTRA_DIST); do \
		if test -f $(DESTDIR)$(MONODOC_SRC_DIR)/$$file; then \
			rm $(DESTDIR)$(MONODOC_SRC_DIR)/$$file; \
		fi; \
	done;
endif

