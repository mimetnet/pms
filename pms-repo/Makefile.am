MFLAGS = -nologo -d:MONO -keyfile:../PMS.snk

ASSEMBLY = pms-repo.exe

REFERENCES =  \
	-r:System.dll \
	-r:System.Data.dll \
	-r:../PMS/PMS.dll \
   -pkg:log4net-1.2
#	-pkg:printgroove-liblog4net2.0

SOURCES = \
	 ./Program.cs \
	 ./Properties/AssemblyInfo.cs

pms_DATA = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(ASSEMBLY)

$(ASSEMBLY): $(SOURCES)
	$(MCS) $(MFLAGS) -target:exe -out:$@ $(REFERENCES) $(SOURCES)
