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

noinst_SCRIPTS = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(SOURCES)

$(ASSEMBLY): $(SOURCES) ../PMS/PMS.dll
	$(MCS) $(MFLAGS) -target:library -out:$@ $(REFERENCES) $(SOURCES)

install-data-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -check_refs -i $(ASSEMBLY)
	$(install_sh) -d $(DESTDIR)$(pmsdir)
	$(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY)

uninstall-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -u $(subst .dll,,$(ASSEMBLY))
	rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY)
