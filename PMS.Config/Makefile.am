MFLAGS = -nologo -d:NET_2_0=MONO -keyfile:../PMS.snk

ASSEMBLY = pms-config.exe

REFERENCES =  \
	-r:System.dll \
	-r:System.Data.dll \
	-r:../PMS/PMS.dll \
	-pkg:printgroove-liblog4net2.0

SOURCES = \
	 ./Program.cs \
	 ./Properties/AssemblyInfo.cs

noinst_SCRIPTS = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY)

$(ASSEMBLY): $(SOURCES)
	$(MCS) $(MFLAGS) -target:exe -out:$@ $(REFERENCES) $(SOURCES)

install-data-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -check_refs -i $(ASSEMBLY)
	$(install_sh) -d $(DESTDIR)$(pmsdir)
	$(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY)

uninstall-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -u $(subst .dll,,$(ASSEMBLY))
	rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY)
