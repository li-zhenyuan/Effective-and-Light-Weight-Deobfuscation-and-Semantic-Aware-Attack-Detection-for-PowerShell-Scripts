 SEt-ItEm ('VAr'+'IAB'+'le:17Ep') ( [type]("{7}{0}{6}{10}{3}{9}{5}{4}{8}{2}{1}"-F'StE','REnCE','FE','MENt.AUTO','.AcTiON','n','m','Sy','pre','matio','.MAnAgE') ) ;  function ADD-s`qL`ROW {
	
	[CmdletBinding()]
	param (
		[Parameter(MAnDatoRY)]
		[string]${CO`MpUT`eRN`AME},
		[Parameter(MaNDAtOry)]
		[string]${DATA`BA`SE},
		[Parameter(mANdatOrY)]
		[string]${t`Able},
		[Parameter(vaLuefROmpIpeLIneBypropErTYnAMe,valuefRomPipeliNe,MAndAtORy)]
		[object]${r`oW},
		[string]${sC`h`EMa} = 'dbo'
	)
	
	begin {
		${err`oRAct`ioNp`ReFeREn`CE} =  (Gi ('vA'+'RiABlE:'+'1'+'7eP')).vALUe::"ST`op"
		.("{4}{1}{0}{2}{3}" -f '-Stri','t','ct','Mode','Se') -Version ("{0}{1}{2}" -f'La','tes','t')
		try {
			${ReQuiR`edM`OdulE} = ("{1}{0}" -f 'LPSX','SQ')
			
			
			
		} catch {
			&("{0}{1}{2}"-f 'W','rite-','Error') ${_}."E`XC`ePT`IOn"."ME`Ssa`gE"
			return
		}
	}
	
	process {
		try {
			${inSE`Rt`StR`InG} = "INSERT INTO $Table ($($Row.PSObject.Properties.Name -join ',')) VALUES ($($Row.PSObject.Properties.Value -join ',')) "
			${i`N`s`erTSTring}
			
		} catch {
			&("{1}{2}{0}{3}"-f'rr','Writ','e-E','or') ${_}."EXCe`pTi`on"."M`essAgE"
		}
	}
}

function GET-S`QL`Row {
	
	[CmdletBinding()]
	param ()
	
	begin {
		${e`RROra`ctio`Npre`FErENcE} =  $17Ep::"st`OP"
		.("{0}{1}{3}{2}" -f'S','et-','de','StrictMo') -Version ("{0}{1}" -f'La','test')
		try {
			
		} catch {
			.("{0}{1}{3}{2}"-f'Writ','e-Er','or','r') ${_}."exCEpT`i`oN"."m`EssagE"
			return
		}
	}
	
	process {
		try {
			
		} catch {
			.("{0}{2}{3}{1}"-f 'W','te-Error','r','i') ${_}."Exce`pT`Ion"."M`eSSAgE"
		}
	}
}

function SET`-S`QlRoW {
	
	[CmdletBinding()]
	param ()
	
	begin {
		${eRROR`ActIO`NpREfE`R`eNce} =  (  IteM ('v'+'a'+'RIAbLe:'+'17Ep')).vAlUe::"st`op"
		&("{0}{4}{2}{3}{1}" -f'Set-','e','tri','ctMod','S') -Version ("{1}{0}" -f'atest','L')
		try {
			
		} catch {
			.("{0}{2}{1}{3}"-f 'Wr','te-Er','i','ror') ${_}."EXC`EP`T`ion"."Mes`sAge"
			return
		}
	}
	
	process {
		try {
			
		} catch {
			&("{2}{1}{0}{3}"-f'te-Erro','i','Wr','r') ${_}."eX`cePT`ion"."mesS`A`Ge"
		}
	}
}

function A`dd`-`CsVROw {
	
	[CmdletBinding()]
	param ()
	
	begin {
		${eRroRACT`IOnp`RE`F`EREnce} =   (  GeT-vaRiaBLE  17EP ).ValuE::"s`TOp"
		&("{1}{2}{3}{0}"-f'ode','Set-','Str','ictM') -Version ("{1}{0}" -f 'est','Lat')
		try {
			
		} catch {
			.("{0}{1}{2}" -f 'Wri','te-E','rror') ${_}."E`xcE`PTiON"."mE`Ssage"
			return
		}
	}
	
	process {
		try {
			
		} catch {
			.("{2}{0}{1}"-f 'r','ite-Error','W') ${_}."exC`EPtI`On"."mEss`Age"
		}
	}
}

function ge`T`-csvroW {
	
	[CmdletBinding(DefaulTpaRaMETersEtnAME = {"{1}{0}"-f 'lRows','Al'})]
	param (
		[Parameter(maNDAtoRy)]
		[ValidateScript({&("{0}{2}{1}{3}" -f 'T','t','es','-Path') -Path ${_} -PathType ("{0}{1}" -f 'Lea','f') })]
		[string]${FIL`E`PAth},
		[Parameter(ManDATORy,pArAmeTerSEtnAmE="SELec`Tr`OWS")]
		[hashtable]${r`OW}
	)
	
	begin {
		${E`Rro`RAc`T`IONp`R`EFerENce} =  (  vARIabLE  17Ep -VAL)::"S`ToP"
		.("{1}{0}{2}{3}" -f 'tM','Set-Stric','o','de') -Version ("{0}{1}" -f'L','atest')
		try {
			
			${V`A`lI`DFIELds} = (&("{2}{0}{1}" -f 'mport-Cs','v','I') ${f`iL`ePATh} | .("{2}{0}{1}" -f'elect-O','bject','S') -First 1)."PSob`JE`CT"."p`Rop`ERTiES"."N`AME"
			if (${vaL`iD`Fi`elDS} -notcontains ${R`Ow}."Ke`yS"[0]) {
				throw "The field name '$($Row.Keys[0])' does not exist in CSV file '$FilePath' "
			}
		} catch {
			&("{0}{3}{1}{2}"-f 'Wri','-E','rror','te') ${_}."e`XCe`pTIOn"."meS`saGe"
			return
		}
	}
	
	process {
		try {
			if (${r`OW}) {
				${Fi`e`lD} = ${R`oW}."k`eyS"[0]
				${VAL`Ue} = ${r`oW}."va`L`UeS"[0]
				.("{0}{2}{1}"-f 'Impo','Csv','rt-') -Path ${f`I`lEPath} | &("{0}{1}{2}{3}"-f 'Wh','er','e-Obj','ect') { ${_}.${FI`eld} -eq ${vaL`Ue} }
			} else {
				.("{1}{0}{2}" -f 't','Impor','-Csv') -Path ${FiL`ePAtH}
			}
		} catch {
			&("{2}{3}{1}{0}" -f 'or','Err','Write','-') ${_}."EXcE`Pt`ioN"."m`EssaGE"
		}
	}
}

function S`et-c`sV`RoW {
	
	[CmdletBinding()]
	param ()
	
	begin {
		${er`RORa`C`TI`ONPRefErENcE} =  (  VaRiaBLe 17Ep  -vALueoN  )::"St`op"
		&("{0}{4}{1}{2}{3}" -f 'Set','tric','tMod','e','-S') -Version ("{1}{0}{2}" -f'a','L','test')
		try {
			
		} catch {
			.("{3}{1}{0}{2}" -f '-E','ite','rror','Wr') ${_}."ex`c`EPtIon"."ME`ss`AGe"
			return
		}
	}
	
	process {
		try {
			
		} catch {
			&("{0}{2}{1}" -f'Write-','r','Erro') ${_}."e`XcEPTIon"."mEsSA`Ge"
		}
	}
}

function A`D`D-exceL`RoW {
	
	[CmdletBinding()]
	param ()
	
	begin {
		${ErRorActIoN`p`R`e`FERenCE} =  ( gET-cHilDiTEM  ('va'+'ria'+'bLe:17Ep')  ).VaLuE::"S`ToP"
		.("{0}{1}{2}{4}{3}" -f'Set-Str','i','ctM','e','od') -Version ("{0}{1}"-f 'L','atest')
		try {
			
		} catch {
			&("{2}{1}{0}"-f 'e-Error','rit','W') ${_}."EX`cEPT`ion"."ME`S`saGE"
			return
		}
	}
	
	process {
		try {
			
		} catch {
			.("{2}{0}{1}" -f'e','-Error','Writ') ${_}."e`xCePti`oN"."mESs`AGe"
		}
	}
}

function G`et-`e`XCelRoW {
	
	[CmdletBinding()]
	param ()
	
	begin {
		${ER`RORAcTi`onPReFE`Re`NCe} =  (  GET-VArIaBLe  17eP  ).vaLUe::"sT`op"
		&("{1}{2}{0}{3}{4}" -f'ri','Set-S','t','c','tMode') -Version ("{0}{2}{1}" -f'Lat','st','e')
		try {
			
		} catch {
			.("{2}{0}{1}" -f'rit','e-Error','W') ${_}."ExCep`T`iOn"."Me`ssagE"
			return
		}
	}
	
	process {
		try {
			
		} catch {
			&("{1}{2}{0}" -f'Error','Writ','e-') ${_}."e`x`CEPTIon"."me`sSA`GE"
		}
	}
}

function S`et`-EX`ceLRow {
	
	[CmdletBinding()]
	param ()
	
	begin {
		${Er`ROr`A`ctIONp`REFerenCE} =  $17eP::"St`op"
		&("{4}{3}{2}{1}{0}"-f'ictMode','r','t','et-S','S') -Version ("{0}{1}" -f'Late','st')
		try {
			
		} catch {
			.("{0}{1}{2}"-f 'Write','-','Error') ${_}."ExceP`Ti`oN"."m`essage"
			return
		}
	}
	
	process {
		try {
			
		} catch {
			.("{0}{2}{1}"-f 'Wri','rror','te-E') ${_}."Ex`ce`ptiOn"."Me`sSagE"
		}
	}
}
