EXTRA_DIST = $(PACKGE).zip $(PACKGE).tree $(PACKGE).source
CLEANFILES = $(PACKGE).zip $(PACKGE).tree $(PACKGE).stamp

if HAVE_MDOC
all: $(PACKGE).stamp

$(PACKGE).stamp: $(builddir)/api/index.xml
	$(MDOC) assemble -o $(PACKAGE) $(builddir)/api
	touch $@

$(builddir)/api/index.xml: $(top_srcdir)/PMS/PMS.dll
	$(MDOC) update -o $(builddir)/api $(top_srcdir)/PMS/PMS.dll

api-html: $(builddir)/api/index.xml
	$(MDOC) export-html -o $(builddir)/api-html $(builddir)/api

install-data-local:
	$(INSTALL) -m 755 -d $(DESTDIR)$(MDOC_SRC_DIR)/
	$(INSTALL_DATA) -t $(DESTDIR)$(MDOC_SRC_DIR)/ $(EXTRA_DIST)

uninstall-local:
	for file in $(EXTRA_DIST); do \
		if test -f $(DESTDIR)$(MDOC_SRC_DIR)/$$file; then \
			rm $(DESTDIR)$(MDOC_SRC_DIR)/$$file; \
		fi; \
	done;
endif

