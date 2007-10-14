MFLAGS = -debug+ -nologo -d:MONO -keyfile:../PMS.snk

ASSEMBLY = PMS.dll

REFERENCES =  \
	-r:System.dll \
	-r:System.Data.dll \
	-pkg:log4net-1.2
#	-pkg:printgroove-liblog4net2.0

SOURCES = \
	 ./PMS/Logger.cs \
	 ./PMS/Config.cs \
	 ./PMS.IO/FileLock.cs \
	 ./PMS.Broker/IPersistenceBroker.cs \
	 ./PMS.Broker/PersistenceBroker.cs \
	 ./PMS.Broker/PersistenceBrokerFactory.cs \
	 ./PMS.Collections.Pool/IObjectPool.cs \
	 ./PMS.Collections.Pool/ObjectPool.cs \
	 ./PMS.Collections.Pool/ManagedObjectPool.cs \
	 ./PMS.Collections.Pool/ObjectNotCheckedOutException.cs \
	 ./PMS.Collections.Pool/ObjectNotInPoolException.cs \
	 ./PMS.Collections.Pool/PrincipalObjectPool.cs \
	 ./PMS.Collections.Pool/ItemCollection.cs \
	 ./PMS.Data/AbstractProvider.cs \
	 ./PMS.Data/DbCommandProxy.cs \
	 ./PMS.Data/DbConnectionProxy.cs \
	 ./PMS.Data/DbDataReaderProxy.cs \
	 ./PMS.Data/DbResult.cs \
	 ./PMS.Data/IDbInspector.cs \
	 ./PMS.Data/IProvider.cs \
	 ./PMS.Data/ProviderDefaultException.cs \
	 ./PMS.Data/ProviderFactory.cs \
	 ./PMS.Data/ProviderNotFoundException.cs \
	 ./PMS.Data.Pool/ConnectionPool.cs \
	 ./PMS.DataAccess/AccessMode.cs \
	 ./PMS.DataAccess/DbEngine.cs \
	 ./PMS.DataAccess/DbEngineNotStartedException.cs \
	 ./PMS.DataAccess/IDbManager.cs \
	 ./PMS.DataAccess/SingleDbManager.cs \
	 ./PMS.Metadata/AssemblyCollection.cs \
	 ./PMS.Metadata/Auto.cs \
	 ./PMS.Metadata/Class.cs \
	 ./PMS.Metadata/ClassCollection.cs \
	 ./PMS.Metadata/ClassNotFoundException.cs \
	 ./PMS.Metadata/Connection.cs \
	 ./PMS.Metadata/ConnectionCollection.cs \
	 ./PMS.Metadata/Field.cs \
	 ./PMS.Metadata/FieldCollection.cs \
	 ./PMS.Metadata/FieldNotFoundException.cs \
	 ./PMS.Metadata/MetaObject.cs \
	 ./PMS.Metadata/Reference.cs \
	 ./PMS.Metadata/Repository.cs \
	 ./PMS.Metadata/RepositoryManager.cs \
	 ./PMS.Metadata/RepositoryNotFoundException.cs \
	 ./PMS.Query/AbstractQuery.cs \
	 ./PMS.Query/AndClause.cs \
	 ./PMS.Query/BetweenClause.cs \
	 ./PMS.Query/Criteria.cs \
	 ./PMS.Query/EqualToClause.cs \
	 ./PMS.Query/GreaterOrEqualToClause.cs \
	 ./PMS.Query/GreaterThanClause.cs \
	 ./PMS.Query/IClause.cs \
	 ./PMS.Query/IQuery.cs \
	 ./PMS.Query/IsNotNullClause.cs \
	 ./PMS.Query/IsNullClause.cs \
	 ./PMS.Query/LessOrEqualToClause.cs \
	 ./PMS.Query/LessThanClause.cs \
	 ./PMS.Query/LikeClause.cs \
	 ./PMS.Query/NotBetweenClause.cs \
	 ./PMS.Query/NotEqualToClause.cs \
	 ./PMS.Query/NotLikeClause.cs \
	 ./PMS.Query/OrClause.cs \
	 ./PMS.Query/QueryByCriteria.cs \
	 ./PMS.Query/QueryByObject.cs \
	 ./PMS.Query/QueryBySql.cs \
	 ./PMS.Query/QueryByType.cs \
	 ./PMS.Query/RangeClause.cs \
	 ./PMS.Query/SqlCommand.cs \
	 ./PMS.Query/ValueClause.cs \
	 ./PMS.Util/TypeLoader.cs \
	 ./PMS.Util/TypeComparer.cs \
	 ./Properties/AssemblyInfo.cs

noinst_SCRIPTS = $(ASSEMBLY)

CLEANFILES = $(ASSEMBLY)

EXTRA_DIST = $(SOURCES) dll.config PMS.csproj PMS.pfx

$(ASSEMBLY): $(SOURCES)
	$(MCS) $(MFLAGS) -target:library -out:$@ $(REFERENCES) $(SOURCES)

install-data-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -check_refs -i $(ASSEMBLY)
	$(install_sh) -d $(DESTDIR)$(pmsdir)
	$(install_sh_DATA) -t $(DESTDIR)$(pmsdir)/ $(ASSEMBLY)

uninstall-local:
	$(GACUTIL) $(GACUTIL_FLAGS) -u $(subst .dll,,$(ASSEMBLY))
	rm $(DESTDIR)$(pmsdir)/$(ASSEMBLY)
