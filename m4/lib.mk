MFLAGS = $(DEBUG_FLAGS) -nologo -d:$(MONO_VERSION) -keyfile:$(top_srcdir)/certs/PMS.snk
#MFLAGS = $(DEBUG_FLAGS) -nologo -d:$(MONO_VERSION) -delaysign+ -keyfile:$(top_srcdir)/certs/PMS.pfx.pub

noinst_SCRIPTS = $(ASSEMBLY)

DEPS_dir = $(top_srcdir)/$(subdir)/.deps
SOURCES_make = $(DEPS_dir)/makefrag
SOURCES_file = $(DEPS_dir)/sources

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb $(SOURCES_file) $(SOURCES_make)
EXTRA_DIST = $(SOURCES) $(subst .dll,.csproj,$(ASSEMBLY))

$(ASSEMBLY): $(SOURCES) $(SOURCES_file)
	$(GMCS) $(MFLAGS) -target:library -out:$@ $(REFERENCES) @$(SOURCES_file)

$(DEPS_dir):
	mkdir $@

$(SOURCES_file): $(DEPS_dir)
	find . -name '*.cs' -type f > $@;

$(SOURCES_make): $(DEPS_dir)
	for f in `find . -name '*.cs' -type f`; do echo $(ASSEMBLY): $$f >> $@; done;

install-data-local:
	$(install_sh) -d $(DESTDIR)$(pmsdir)
	$(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY) 
	if test -n "$(DEBUG_FLAGS)"; then $(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY).mdb; fi;
	$(GACUTIL) -i "$(DESTDIR)$(pmsdir)/$(ASSEMBLY)" $(GACUTIL_FLAGS) -check_refs -f

uninstall-local:
	$(GACUTIL) -us $(DESTDIR)$(pmsdir)/$(ASSEMBLY) $(GACUTIL_FLAGS) 
	rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY) 
	if test -n "$(DEBUG_FLAGS)"; then rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY).mdb; fi;

-include $(SOURCES_make)
