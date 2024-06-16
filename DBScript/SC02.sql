if not exists(select syscolumns.id from syscolumns join sysobjects on  syscolumns.ID = sysobjects.ID  where  sysobjects.name = 'TPL_TRUCKPLAN' and  syscolumns.name = 'TPL_PCOLOR') begin
	ALTER TABLE TPL_TRUCKPLAN ADD TPL_PCOLOR int default -1;
end
go


if not exists(select syscolumns.id from syscolumns join sysobjects on  syscolumns.ID = sysobjects.ID  where  sysobjects.name = 'TPL_TRUCKPLAN' and  syscolumns.name = 'TPL_PSELECT') begin
	ALTER TABLE TPL_TRUCKPLAN ADD TPL_PSELECT TY_BVALUE default -1;
end
go

update TPL_TRUCKPLAN set TPL_PSELECT = -1, TPL_PCOLOR = -1
go
