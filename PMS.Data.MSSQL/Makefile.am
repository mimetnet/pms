AUTOMAKE_OPTIONS = gnu

if HAVE_SQLITE
include $(top_srcdir)/m4/lib.mk
endif

ASSEMBLY = PMS.Data.MSSQL.dll

REFERENCES =  \
	-r:System.dll \
	-r:System.Data.dll \
	-r:$(top_srcdir)/PMS/PMS.dll \
	$(LOG4NET_LIBS)
