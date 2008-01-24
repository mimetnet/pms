MFLAGS = $(DEBUG_FLAGS) -nologo -d:MONO -keyfile:$(top_srcdir)/PMS.snk

ASSEMBLY = PMS.Data.Pgsql.dll

REFERENCES =  \
	-r:System.dll \
	-r:System.Data.dll \
	-r:Npgsql.dll \
	-r:../PMS/PMS.dll \
	-pkg:$(PKGLOG4NET)

SOURCES = \
	 ./PMS.Data.Pgsql/PostgresqlInspector.cs \
	 ./PMS.Data.Pgsql/PostgresqlProvider.cs \
	 ./Properties/AssemblyInfo.cs

noinst_SCRIPTS = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(SOURCES)

$(ASSEMBLY): $(SOURCES) ../PMS/PMS.dll
	$(MCS) $(MFLAGS) -target:library -out:$@ $(REFERENCES) $(SOURCES)

install-data-local:
	$(install_sh) -d $(DESTDIR)$(pmsdir)
	$(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY) 
	if test -n "$(DEBUG_FLAGS)"; then $(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY).mdb; fi;
	$(GACUTIL) -i "$(DESTDIR)$(pmsdir)/$(ASSEMBLY)" $(GACUTIL_FLAGS) -check_refs -f

uninstall-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -us $(DESTDIR)$(pmsdir)/$(ASSEMBLY)
	rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY) 
	if test -n "$(DEBUG_FLAGS)"; then $(DESTDIR)$(pmsdir)/$(ASSEMBLY).mdb; fi;
