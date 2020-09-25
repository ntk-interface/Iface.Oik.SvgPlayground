logMessage                         - вывести сообщение для отладки

setElementProperty                 - установить значение свойства элемента SVG
getElementProperty                 - получить значение свойства элемента SVG

initTmStatus                       - инициализировать телесигнал
initTmAnalog                       - инициализировать телеизмерение
initVariable                       - инициализировать внутреннюю переменную схемы

getTmStatusStatus                  - получить состояние телесигнала (0 или 1)
getTmStatusCaption                 - получить текстовое состояние телесигнала (типа вкл или откл)
isTmStatusUnreliable               - является телесигнал недостоверным
isTmStatusMalfunction              - является ли состояние телесигнала неиспранвым
isTmStatusIntermediate             - является ли состояние телесигнала промежуточным
isTmStatusManuallySet              - является ли состояние телесигнала установленным вручную
isTmStatusOn                       - является ли состояние телесигнала включенным

getTmAnalogValue                   - получить значение телеизмерения в виде числа
getTmAnalogUnit                    - получить единицу измерения телеизмерения
getTmAnalogValueString             - получить значение телеизмерения в виде строки (значение)
getTmAnalogValueWithUnitString     - получить значение телеизмерения в виде строки (значение + единица измерения)
isTmAnalogUnreliable               - является ли значение телеизмерения недостоверным
isTmAnalogAlarmAlert               - является ли значение телеизмерения со взведенной аварийной уставкой
isTmAnalogAlarmWarning             - является ли значение телеизмерения со взведенной предупредительной уставкой

isVariableUnreliable               - является ли состояние внутренней переменной недостоверным
isVariableManuallySet              - является ли состояние внутренней переменной установленным вручную
isVariableOn                       - является ли состояние внутренней переменной включенным
setVariable                        - установить значение внутренней переменной
setVariableUnreliable              - установить признак недостоверности внутренней переменной
setVariableManuallySet             - установить значение ручной установки внутренней переменной

initTmStatusDefaultCommands        - инициализировать команды по умолчанию для телесигнала
initTmAnalogDefaultCommands        - инициализировать команды по умолчанию для телеизмерения
initOpenSvgDefaultCommands