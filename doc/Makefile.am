SUBDIRS = man

EXTRA_DIST = pms.zip pms.tree pms.source
CLEANFILES = pms.zip pms.tree pms.stamp

if HAVE_MONODOC
all: pms.stamp

pms.stamp: api/index.xml
	$(MDASSEMBLER) --ecma api -o pms
	touch $@

api/index.xml: $(top_srcdir)/PMS/PMS.dll
	$(MONODOCER) -assembly:$< -name:$(PACKAGE) -path:api -pretty

api-html:
	$(MONODOCS2HTML) --source api --dest api-html

install-data-local:
	$(install_sh_DATA) -m 755 -d $(DESTDIR)$(MONODOC_SRC_DIR)/
	$(install_sh_DATA) -t $(DESTDIR)$(MONODOC_SRC_DIR)/ $(EXTRA_DIST)

uninstall-local:
	for file in $(EXTRA_DIST); do \
		if test -f $(DESTDIR)$(MONODOC_SRC_DIR)/$$file; then \
			rm $(DESTDIR)$(MONODOC_SRC_DIR)/$$file; \
		fi; \
	done;
endif

