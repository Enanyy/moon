﻿
--FUNCTION_CODE_BRGIN
local function f(key,ch,en) return {key = key,ch = ch,en = en} end 
--FUNCTION_CODE_END
local M =
{
    Data=
    {
--DATA_CODE_BEGIN
		['TestName1'] = f('TestName1','fqwfqwf\'asd','fqwfqwf\'asd'),
		['TestName2'] = f('TestName2','中文\'','中文\''),
		['TestName3'] = f('TestName3','特殊字符/ // \ \\ /" /"/" . ,a','特殊字符/ // \ \\ /" /"/" . ,a'),
		['TestName5'] = f('TestName5','哈哈哈哈1','哈哈哈哈1'),
		['TestName6'] = f('TestName6','哈哈哈哈2','哈哈哈哈2'),
		['TestName7'] = f('TestName7','哈哈哈哈3','哈哈哈哈3'),
		['TestName8'] = f('TestName8','哈哈哈哈4','哈哈哈哈4'),
		['TestName9'] = f('TestName9','哈哈哈哈5','哈哈哈哈5'),
		['TestName10'] = f('TestName10','哈哈哈哈6','哈哈哈哈6'),
		['TestName11'] = f('TestName11','哈哈哈哈7','哈哈哈哈7'),
		['TestName12'] = f('TestName12','哈哈哈哈8','哈哈哈哈8'),
		['TestName13'] = f('TestName13','哈哈哈哈9','哈哈哈哈9'),
		['TestName14'] = f('TestName14','哈哈哈哈10','哈哈哈哈10'),
		['TestName15'] = f('TestName15','哈哈哈哈11','哈哈哈哈11'),
		['TestName16'] = f('TestName16','哈哈哈哈12','哈哈哈哈12'),
		['TestName17'] = f('TestName17','哈哈哈哈13','哈哈哈哈13'),
		['TestName18'] = f('TestName18','哈哈哈哈14','哈哈哈哈14'),
		['TestName19'] = f('TestName19','哈哈哈哈15','哈哈哈哈15'),
		['TestName20'] = f('TestName20','哈哈哈哈16','哈哈哈哈16'),
		['TestName21'] = f('TestName21','哈哈哈哈17','哈哈哈哈17'),
		['TestName22'] = f('TestName22','哈哈哈哈18','哈哈哈哈18'),
		['TestName23'] = f('TestName23','哈哈哈哈19','哈哈哈哈19'),
		['TestName24'] = f('TestName24','哈哈哈哈20','哈哈哈哈20'),
		['TestName25'] = f('TestName25','哈哈哈哈21','哈哈哈哈21'),
		['TestName26'] = f('TestName26','哈哈哈哈22','哈哈哈哈22'),
		['TestName27'] = f('TestName27','哈哈哈哈23','哈哈哈哈23'),
		['TestName28'] = f('TestName28','哈哈哈哈24','哈哈哈哈24'),
		['TestName29'] = f('TestName29','哈哈哈哈25','哈哈哈哈25'),
		['TestName30'] = f('TestName30','哈哈哈哈26','哈哈哈哈26'),
		['TestName31'] = f('TestName31','哈哈哈哈27','哈哈哈哈27'),
		['TestName32'] = f('TestName32','哈哈哈哈28','哈哈哈哈28'),
		['TestName33'] = f('TestName33','哈哈哈哈29','哈哈哈哈29'),
		['TestName34'] = f('TestName34','哈哈哈哈30','哈哈哈哈30'),
		['TestName35'] = f('TestName35','哈哈哈哈31','哈哈哈哈31'),
		['TestName36'] = f('TestName36','哈哈哈哈32','哈哈哈哈32'),
		['TestName37'] = f('TestName37','哈哈哈哈33','哈哈哈哈33'),
		['TestName38'] = f('TestName38','哈哈哈哈34','哈哈哈哈34'),
		['TestName39'] = f('TestName39','哈哈哈哈35','哈哈哈哈35'),
		['TestName40'] = f('TestName40','哈哈哈哈36','哈哈哈哈36'),
		['TestName41'] = f('TestName41','哈哈哈哈37','哈哈哈哈37'),
		['TestName42'] = f('TestName42','哈哈哈哈38','哈哈哈哈38'),
		['TestName43'] = f('TestName43','哈哈哈哈39','哈哈哈哈39'),
		['TestName44'] = f('TestName44','哈哈哈哈40','哈哈哈哈40'),
		['TestName45'] = f('TestName45','哈哈哈哈41','哈哈哈哈41'),
		['TestName46'] = f('TestName46','哈哈哈哈42','哈哈哈哈42'),
		['TestName47'] = f('TestName47','哈哈哈哈43','哈哈哈哈43'),
		['TestName48'] = f('TestName48','哈哈哈哈44','哈哈哈哈44'),
		['TestName49'] = f('TestName49','哈哈哈哈45','哈哈哈哈45'),
		['TestName50'] = f('TestName50','哈哈哈哈46','哈哈哈哈46'),
		['TestName51'] = f('TestName51','哈哈哈哈47','哈哈哈哈47'),
		['TestName52'] = f('TestName52','哈哈哈哈48','哈哈哈哈48'),
		['TestName53'] = f('TestName53','哈哈哈哈49','哈哈哈哈49'),
		['TestName54'] = f('TestName54','哈哈哈哈50','哈哈哈哈50'),
		['TestName55'] = f('TestName55','哈哈哈哈51','哈哈哈哈51'),
		['TestName56'] = f('TestName56','哈哈哈哈52','哈哈哈哈52'),
		['TestName57'] = f('TestName57','哈哈哈哈53','哈哈哈哈53'),
		['TestName58'] = f('TestName58','哈哈哈哈54','哈哈哈哈54'),
		['TestName59'] = f('TestName59','哈哈哈哈55','哈哈哈哈55'),
		['TestName60'] = f('TestName60','哈哈哈哈56','哈哈哈哈56'),
		['TestName61'] = f('TestName61','哈哈哈哈57','哈哈哈哈57'),
		['TestName62'] = f('TestName62','哈哈哈哈58','哈哈哈哈58'),
		['TestName63'] = f('TestName63','哈哈哈哈59','哈哈哈哈59'),
		['TestName64'] = f('TestName64','哈哈哈哈60','哈哈哈哈60'),
		['TestName65'] = f('TestName65','哈哈哈哈61','哈哈哈哈61'),
		['TestName66'] = f('TestName66','哈哈哈哈62','哈哈哈哈62'),
		['TestName67'] = f('TestName67','哈哈哈哈63','哈哈哈哈63'),
		['TestName68'] = f('TestName68','哈哈哈哈64','哈哈哈哈64'),
		['TestName69'] = f('TestName69','哈哈哈哈65','哈哈哈哈65'),
		['TestName70'] = f('TestName70','哈哈哈哈66','哈哈哈哈66'),
		['TestName71'] = f('TestName71','哈哈哈哈67','哈哈哈哈67'),
		['TestName72'] = f('TestName72','哈哈哈哈68','哈哈哈哈68'),
		['TestName73'] = f('TestName73','哈哈哈哈69','哈哈哈哈69'),
		['TestName74'] = f('TestName74','哈哈哈哈70','哈哈哈哈70'),
		['TestName75'] = f('TestName75','哈哈哈哈71','哈哈哈哈71'),
		['TestName76'] = f('TestName76','哈哈哈哈72','哈哈哈哈72'),
		['TestName77'] = f('TestName77','哈哈哈哈73','哈哈哈哈73'),
		['TestName78'] = f('TestName78','哈哈哈哈74','哈哈哈哈74'),
		['TestName79'] = f('TestName79','哈哈哈哈75','哈哈哈哈75'),
		['TestName80'] = f('TestName80','哈哈哈哈76','哈哈哈哈76'),
		['TestName81'] = f('TestName81','哈哈哈哈77','哈哈哈哈77'),
		['TestName82'] = f('TestName82','哈哈哈哈78','哈哈哈哈78'),
		['TestName83'] = f('TestName83','哈哈哈哈79','哈哈哈哈79'),
		['TestName84'] = f('TestName84','哈哈哈哈80','哈哈哈哈80'),
		['TestName85'] = f('TestName85','哈哈哈哈81','哈哈哈哈81'),
		['TestName86'] = f('TestName86','哈哈哈哈82','哈哈哈哈82'),
		['TestName87'] = f('TestName87','哈哈哈哈83','哈哈哈哈83'),
		['TestName88'] = f('TestName88','哈哈哈哈84','哈哈哈哈84'),
		['TestName89'] = f('TestName89','哈哈哈哈85','哈哈哈哈85'),
		['TestName90'] = f('TestName90','哈哈哈哈86','哈哈哈哈86'),
		['TestName91'] = f('TestName91','哈哈哈哈87','哈哈哈哈87'),
		['TestName92'] = f('TestName92','哈哈哈哈88','哈哈哈哈88'),
		['TestName93'] = f('TestName93','哈哈哈哈89','哈哈哈哈89'),
		['TestName94'] = f('TestName94','哈哈哈哈90','哈哈哈哈90'),
		['TestName95'] = f('TestName95','哈哈哈哈91','哈哈哈哈91'),
		['TestName96'] = f('TestName96','哈哈哈哈92','哈哈哈哈92'),
		['TestName97'] = f('TestName97','哈哈哈哈93','哈哈哈哈93'),
		['TestName98'] = f('TestName98','哈哈哈哈94','哈哈哈哈94'),
		['TestName99'] = f('TestName99','哈哈哈哈95','哈哈哈哈95'),
		['TestName100'] = f('TestName100','哈哈哈哈96','哈哈哈哈96'),
		['TestName101'] = f('TestName101','哈哈哈哈97','哈哈哈哈97'),
		['TestName102'] = f('TestName102','哈哈哈哈98','哈哈哈哈98'),
		['TestName103'] = f('TestName103','哈哈哈哈99','哈哈哈哈99'),
		['TestName104'] = f('TestName104','哈哈哈哈100','哈哈哈哈100'),
		['TestName105'] = f('TestName105','哈哈哈哈101','哈哈哈哈101'),
		['TestName106'] = f('TestName106','哈哈哈哈102','哈哈哈哈102'),
		['TestName107'] = f('TestName107','哈哈哈哈103','哈哈哈哈103'),
		['TestName108'] = f('TestName108','哈哈哈哈104','哈哈哈哈104'),
		['TestName109'] = f('TestName109','哈哈哈哈105','哈哈哈哈105'),
		['TestName110'] = f('TestName110','哈哈哈哈106','哈哈哈哈106'),
		['TestName111'] = f('TestName111','哈哈哈哈107','哈哈哈哈107'),
		['TestName112'] = f('TestName112','哈哈哈哈108','哈哈哈哈108'),
		['TestName113'] = f('TestName113','哈哈哈哈109','哈哈哈哈109'),
		['TestName114'] = f('TestName114','哈哈哈哈110','哈哈哈哈110'),
		['TestName115'] = f('TestName115','哈哈哈哈111','哈哈哈哈111'),
		['TestName116'] = f('TestName116','哈哈哈哈112','哈哈哈哈112'),
		['TestName117'] = f('TestName117','哈哈哈哈113','哈哈哈哈113'),
		['TestName118'] = f('TestName118','哈哈哈哈114','哈哈哈哈114'),
		['TestName119'] = f('TestName119','哈哈哈哈115','哈哈哈哈115'),
		['TestName120'] = f('TestName120','哈哈哈哈116','哈哈哈哈116'),
		['TestName121'] = f('TestName121','哈哈哈哈117','哈哈哈哈117'),
		['TestName122'] = f('TestName122','哈哈哈哈118','哈哈哈哈118'),
		['TestName123'] = f('TestName123','哈哈哈哈119','哈哈哈哈119'),
		['TestName124'] = f('TestName124','哈哈哈哈120','哈哈哈哈120'),
		['TestName125'] = f('TestName125','哈哈哈哈121','哈哈哈哈121'),
		['TestName126'] = f('TestName126','哈哈哈哈122','哈哈哈哈122'),
		['TestName127'] = f('TestName127','哈哈哈哈123','哈哈哈哈123'),
		['TestName128'] = f('TestName128','哈哈哈哈124','哈哈哈哈124'),
		['TestName129'] = f('TestName129','哈哈哈哈125','哈哈哈哈125'),
		['TestName130'] = f('TestName130','哈哈哈哈126','哈哈哈哈126'),
		['TestName131'] = f('TestName131','哈哈哈哈127','哈哈哈哈127'),
		['TestName132'] = f('TestName132','哈哈哈哈128','哈哈哈哈128'),
		['TestName133'] = f('TestName133','哈哈哈哈129','哈哈哈哈129'),
		['TestName134'] = f('TestName134','哈哈哈哈130','哈哈哈哈130'),
		['TestName135'] = f('TestName135','哈哈哈哈131','哈哈哈哈131'),
		['TestName136'] = f('TestName136','哈哈哈哈132','哈哈哈哈132'),
		['TestName137'] = f('TestName137','哈哈哈哈133','哈哈哈哈133'),
		['TestName138'] = f('TestName138','哈哈哈哈134','哈哈哈哈134'),
		['TestName139'] = f('TestName139','哈哈哈哈135','哈哈哈哈135'),
		['TestName140'] = f('TestName140','哈哈哈哈136','哈哈哈哈136'),
		['TestName141'] = f('TestName141','哈哈哈哈137','哈哈哈哈137'),
		['TestName142'] = f('TestName142','哈哈哈哈138','哈哈哈哈138'),
		['TestName143'] = f('TestName143','哈哈哈哈139','哈哈哈哈139'),
		['TestName144'] = f('TestName144','哈哈哈哈140','哈哈哈哈140'),
		['TestName145'] = f('TestName145','哈哈哈哈141','哈哈哈哈141'),
		['TestName146'] = f('TestName146','哈哈哈哈142','哈哈哈哈142'),
		['TestName147'] = f('TestName147','哈哈哈哈143','哈哈哈哈143'),
		['TestName148'] = f('TestName148','哈哈哈哈144','哈哈哈哈144'),
		['TestName149'] = f('TestName149','哈哈哈哈145','哈哈哈哈145'),
		['TestName150'] = f('TestName150','哈哈哈哈146','哈哈哈哈146'),
		['TestName151'] = f('TestName151','哈哈哈哈147','哈哈哈哈147'),
		['TestName152'] = f('TestName152','哈哈哈哈148','哈哈哈哈148'),
		['TestName153'] = f('TestName153','哈哈哈哈149','哈哈哈哈149'),
		['TestName154'] = f('TestName154','哈哈哈哈150','哈哈哈哈150'),
		['TestName155'] = f('TestName155','哈哈哈哈151','哈哈哈哈151'),
		['TestName156'] = f('TestName156','哈哈哈哈152','哈哈哈哈152'),
		['TestName157'] = f('TestName157','哈哈哈哈153','哈哈哈哈153'),
		['TestName158'] = f('TestName158','哈哈哈哈154','哈哈哈哈154'),
		['TestName159'] = f('TestName159','哈哈哈哈155','哈哈哈哈155'),
		['TestName160'] = f('TestName160','哈哈哈哈156','哈哈哈哈156'),
		['TestName161'] = f('TestName161','哈哈哈哈157','哈哈哈哈157'),
		['TestName162'] = f('TestName162','哈哈哈哈158','哈哈哈哈158'),
		['TestName163'] = f('TestName163','哈哈哈哈159','哈哈哈哈159'),
		['TestName164'] = f('TestName164','哈哈哈哈160','哈哈哈哈160'),
		['TestName165'] = f('TestName165','哈哈哈哈161','哈哈哈哈161'),
		['TestName166'] = f('TestName166','哈哈哈哈162','哈哈哈哈162'),
		['TestName167'] = f('TestName167','哈哈哈哈163','哈哈哈哈163'),
		['TestName168'] = f('TestName168','哈哈哈哈164','哈哈哈哈164'),
		['TestName169'] = f('TestName169','哈哈哈哈165','哈哈哈哈165'),
		['TestName170'] = f('TestName170','哈哈哈哈166','哈哈哈哈166'),
		['TestName171'] = f('TestName171','哈哈哈哈167','哈哈哈哈167'),
		['TestName172'] = f('TestName172','哈哈哈哈168','哈哈哈哈168'),
		['TestName173'] = f('TestName173','哈哈哈哈169','哈哈哈哈169'),
		['TestName174'] = f('TestName174','哈哈哈哈170','哈哈哈哈170'),
		['TestName175'] = f('TestName175','哈哈哈哈171','哈哈哈哈171'),
		['TestName176'] = f('TestName176','哈哈哈哈172','哈哈哈哈172'),
		['TestName177'] = f('TestName177','哈哈哈哈173','哈哈哈哈173'),
		['TestName178'] = f('TestName178','哈哈哈哈174','哈哈哈哈174'),
		['TestName179'] = f('TestName179','哈哈哈哈175','哈哈哈哈175'),
		['TestName180'] = f('TestName180','哈哈哈哈176','哈哈哈哈176'),
		['TestName181'] = f('TestName181','哈哈哈哈177','哈哈哈哈177'),
		['TestName182'] = f('TestName182','哈哈哈哈178','哈哈哈哈178'),
		['TestName183'] = f('TestName183','哈哈哈哈179','哈哈哈哈179'),
		['TestName184'] = f('TestName184','哈哈哈哈180','哈哈哈哈180'),
		['TestName185'] = f('TestName185','哈哈哈哈181','哈哈哈哈181'),
		['TestName186'] = f('TestName186','哈哈哈哈182','哈哈哈哈182'),
		['TestName187'] = f('TestName187','哈哈哈哈183','哈哈哈哈183'),
		['TestName188'] = f('TestName188','哈哈哈哈184','哈哈哈哈184'),
		['TestName189'] = f('TestName189','哈哈哈哈185','哈哈哈哈185'),
		['TestName190'] = f('TestName190','哈哈哈哈186','哈哈哈哈186'),
		['TestName191'] = f('TestName191','哈哈哈哈187','哈哈哈哈187'),
		['TestName192'] = f('TestName192','哈哈哈哈188','哈哈哈哈188'),
		['TestName193'] = f('TestName193','哈哈哈哈189','哈哈哈哈189'),
		['TestName194'] = f('TestName194','哈哈哈哈190','哈哈哈哈190'),
		['TestName195'] = f('TestName195','哈哈哈哈191','哈哈哈哈191'),
		['TestName196'] = f('TestName196','哈哈哈哈192','哈哈哈哈192'),
		['TestName197'] = f('TestName197','哈哈哈哈193','哈哈哈哈193'),
		['TestName198'] = f('TestName198','哈哈哈哈194','哈哈哈哈194'),
		['TestName199'] = f('TestName199','哈哈哈哈195','哈哈哈哈195'),
		['TestName200'] = f('TestName200','哈哈哈哈196','哈哈哈哈196'),

--DATA_CODE_END
    }
}

function M.Get(id)
    return M.Data[id]
end

return M