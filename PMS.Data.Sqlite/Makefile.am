MFLAGS = $(DEBUG_FLAGS) -nologo -d:MONO -keyfile:$(top_srcdir)/PMS.snk

ASSEMBLY = PMS.Data.Sqlite.dll

REFERENCES =  \
	-r:System.dll \
	-r:System.Data.dll \
	-r:Mono.Data.Sqlite.dll \
	-r:../PMS/PMS.dll \
	-pkg:$(PKGLOG4NET)

SOURCES = \
	 ./PMS.Data.Sqlite/SqliteInspector.cs \
	 ./PMS.Data.Sqlite/SqliteProvider.cs \
	 ./Properties/AssemblyInfo.cs

if HAVE_SQLITE
noinst_SCRIPTS = $(ASSEMBLY)
endif

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(SOURCES)

$(ASSEMBLY): $(SOURCES) ../PMS/PMS.dll
	$(MCS) $(MFLAGS) -target:library -out:$@ $(REFERENCES) $(SOURCES)

if HAVE_SQLITE
install-data-local:
	$(install_sh) -d $(DESTDIR)$(pmsdir)
	$(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY) 
	if test -n "$(DEBUG_FLAGS)"; then $(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY).mdb; fi;
	$(GACUTIL) -i "$(DESTDIR)$(pmsdir)/$(ASSEMBLY)" $(GACUTIL_FLAGS) -check_refs -f
endif

uninstall-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -us $(DESTDIR)$(pmsdir)/$(ASSEMBLY)
	rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY) 
	if test -n "$(DEBUG_FLAGS)"; then $(DESTDIR)$(pmsdir)/$(ASSEMBLY).mdb; fi;
