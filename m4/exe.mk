MFLAGS = $(DEBUG_FLAGS) -nologo -d:$(MONO_VERSION) -keyfile:$(top_srcdir)/certs/PMS.snk
#MFLAGS = $(DEBUG_FLAGS) -nologo -d:$(MONO_VERSION) -delaysign+ -keyfile:$(top_srcdir)/certs/PMS.pfx.pub

noinst_SCRIPTS = $(ASSEMBLY)

NULL =
DEPS_dir = $(builddir)/.deps
SOURCES_make = $(DEPS_dir)/makefrag
SOURCES_file = $(DEPS_dir)/sources

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb $(SOURCES_file) $(SOURCES_make)
EXTRA_DIST = $(SOURCES) $(subst .exe,.csproj,$(ASSEMBLY))

$(ASSEMBLY): $(SOURCES) $(SOURCES_file)
	$(GMCS) $(MFLAGS) -target:exe -out:$@ $(REFERENCES) @$(SOURCES_file)

$(DEPS_dir):
	mkdir $@

$(SOURCES_file): $(DEPS_dir)
	find $(srcdir) -name '*.cs' -type f > $@;

$(SOURCES_make): $(DEPS_dir)
	for f in `find $(srcdir) -name '*.cs' -type f`; do echo $(ASSEMBLY): $$f >> $@; done;

install-data-local:
	$(INSTALL) -d $(DESTDIR)$(pmsdir)
	$(INSTALL_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY)
	if test -n "$(DEBUG_FLAGS)"; then $(INSTALL_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY).mdb; fi;

uninstall-local:
	rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY)
	if test -n "$(DEBUG_FLAGS)"; then rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY).mdb; fi;

-include $(SOURCES_make)
