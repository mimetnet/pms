ass = PMS.NUnit.dll

LIBRARIES =  \
		-pkg:nunit \
		-pkg:printgroove-liblog4net2.0 \
		-r:Npgsql \
		-r:System \
		-r:System.Data \
		-r:../PMS/bin/PMS.dll \
		-r:../PMS.NUnit.Model/bin/PMS.NUnit.Model.dll

-include ../build/rules.mk

nunit:
	cp ../PMS/bin/PMS.dll ./bin
	cp ../PMS.NUnit.Model/bin/PMS.NUnit.Model.dll ./bin


