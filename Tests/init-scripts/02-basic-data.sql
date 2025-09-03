--
-- PostgreSQL database dump - Basic configuration data
--

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
00000000000000_CreateIdentitySchema	2.2.6-servicing-10079
20190624021208_AddClientInitialData	2.2.6-servicing-10079
20190626015549_AddCountryStateDistrictCity	2.2.6-servicing-10079
20190627151559_AddApplicationUsersInitialFields	2.2.6-servicing-10079
20190701040228_AddedModificationsToAppUser	2.2.6-servicing-10079
20190701054849_AddedClientsFieldsAndRelatedEntities	2.2.6-servicing-10079
20190703152734_AddApplicationRole	2.2.6-servicing-10079
20190704212001_AddRefreshTokens	2.2.6-servicing-10079
20190705152557_AddedTaxTypeDynamizationPerCountryAndRemovedHardTables	2.2.6-servicing-10079
20190705192109_AddRelationForClientsAndCustomAccountabilityTypesPerCountry	2.2.6-servicing-10079
20190706114203_AddProductsAndRelatedEntities	2.2.6-servicing-10079
20190712140931_AddContrats	2.2.6-servicing-10079
20190726011736_AddIsIdentificationFieldToTaxType	2.2.6-servicing-10079
20191211154617_ContractModel	2.2.6-servicing-10079
20191217143927_Cambios contratos y ordenes de publicacion	2.2.6-servicing-10079
20191218115524_AddSellerToOP	2.2.6-servicing-10079
20191223192816_Currency	2.2.6-servicing-10079
20200108120634_AddObservationToContract	2.2.6-servicing-10079
20200108140724_AddSoldSpacetoOP	2.2.6-servicing-10079
20200113181516_addVolumeDiscount	2.2.6-servicing-10079
20200116150451_addLocationDiscount	2.2.6-servicing-10079
20200121173812_desnormalizarCurrencyParity	2.2.6-servicing-10079
20200122131251_changeClientTaxesToClient	2.2.6-servicing-10079
20200122132007_changeClientTaxesToTypeTax	2.2.6-servicing-10079
20200122142018_addContactToClient	2.2.6-servicing-10079
20200123162911_addCodigoTelefonico	2.2.6-servicing-10079
20200124125450_addLogicDelete	2.2.6-servicing-10079
20200128122723_desnormaliceLocationToClient	2.2.6-servicing-10079
20200128132450_nullableColumnsClient	2.2.6-servicing-10079
20200129134349_addContractHistorical	2.2.6-servicing-10079
20200221152558_creationDateToOP	2.2.6-servicing-10079
20200221161611_creationAndLastUpdateDateToOP	2.2.6-servicing-10079
20200221162753_AddReporOPForProductionExport	2.2.6-servicing-10079
20200226122547_addIVAProducto	2.2.6-servicing-10079
20200226140428_AddIVAContract	2.2.6-servicing-10079
20200307122706_Auditory	2.2.6-servicing-10079
20200317151257_Currency_deleted	2.2.6-servicing-10079
20200410154202_MigrateTextToCitext	2.2.6-servicing-10079
20201007180127_PageNumber	2.2.6-servicing-10079
20201014145319_UnitPriceWithDiscounts	2.2.6-servicing-10079
20221221115211_FixSoldSpaceBalance	2.2.6-servicing-10079
20221229174615_DeleteContractInconsistent	2.2.6-servicing-10079
20230427124007_AddClosedColumnInProduct	2.2.6-servicing-10079
20231023201336_ShowProductAdvertisingSpace	2.2.6-servicing-10079
20231122200950_FixSellersPublishingOrders	2.2.6-servicing-10079
20231123145946_FixSellersPublishingOrders2	2.2.6-servicing-10079
20231127144741_AddDiscountFixed	2.2.6-servicing-10079
20231127150549_MigreateDiscountsFixedValues	2.2.6-servicing-10079
20231128114044_AddDiscountFixed2	2.2.6-servicing-10079
20231128120023_MigreateDiscountsFixedValues2	2.2.6-servicing-10079
20231129131332_AddDiscountByVolume	2.2.6-servicing-10079
20231129152051_MigreateDiscountsByVolumeValues	2.2.6-servicing-10079
20231129152753_AddDiscountByVolumeSoldSpace	2.2.6-servicing-10079
20231129152908_MigreateDiscountsByVolumeValuesSoldSpace	2.2.6-servicing-10079
20231130024648_AddDiscountByLocationSoldSpace	2.2.6-servicing-10079
20231130114655_MigreateDiscountsByVLocationValuesSoldSpace	2.2.6-servicing-10079
20250108031443_AddCurrencyParityTable	8.0.12
20250108041028_MigrateCurrenciesValuesFromProducts	8.0.12
20250115142425_AddEuroParity	8.0.12
20250115173720_ChangeColumnNameEuroParity	8.0.12
20250116025142_UseEuroColumnInCurrencyTable	8.0.12
20250116172341_UseEuroColumnRenameInCurrencyTable	8.0.12
20250127165712_XubioIdClient	8.0.12
20250127235556_TaxCategoryTable	8.0.12
20250128133850_AddXubioCodeCountry	8.0.12
20250128141203_AddXubioCodeState	8.0.12
20250129004355_AddXubioCodeCity	8.0.12
20250211201508_ContractUseEuro	8.0.12
20250218192520_AddCurrencyParities	8.0.12
20250221204010_AddXubioProducCodeProductTable	8.0.12
20250428203158_AddXubioInvoiceIdSoldSpaces	8.0.12
20250505221555_FormatArgentinaIdentificationValues	8.0.12
20250515144101_AddInvoiceNumberSpaceSold	8.0.12
20250515164942_AddInvoiceNumberPublishingOrder	8.0.12
20250515172517_RemoveXubioInvoiceId	8.0.12
20250728220533_DataTasks	8.0.12
20250812130638_TrasactionIdContractAndOrders	8.0.12
\.
