MFLAGS = -debug+ -nologo -d:NET_2_0=MONO -keyfile:$(top_srcdir)/PMS.snk

ASSEMBLY = PMS.Data.Pgsql.dll

REFERENCES =  \
	-r:System.dll \
	-r:System.Data.dll \
	-r:Npgsql.dll \
	-r:../PMS/PMS.dll \
	-pkg:printgroove-liblog4net2.0

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
	$(GACUTIL) $(GACUTIL_FLAGS) -check_refs -i $(ASSEMBLY)
	$(install_sh) -d $(DESTDIR)$(pmsdir)
	$(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY)

uninstall-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -u $(subst .dll,,$(ASSEMBLY))
	rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY)
