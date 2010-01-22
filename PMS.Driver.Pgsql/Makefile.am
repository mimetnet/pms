AUTOMAKE_OPTIONS = gnu

if HAVE_NPGSQL
include $(top_srcdir)/m4/lib.mk
endif

ASSEMBLY = PMS.Driver.Pgsql.dll

REFERENCES =  \
	-r:System \
	-r:System.Data \
	-r:Npgsql \
	-r:$(top_srcdir)/PMS/PMS.dll \
	$(LOG4NET_LIBS) \
	$(NULL)
