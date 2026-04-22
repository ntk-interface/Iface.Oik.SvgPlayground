## Iface.Oik.SvgPlayground

Программа для тестирования SVG-схем, предназначенных для каталога документов ПО Клиент10 «ОИК Диспетчер НТ»

Программа не требует связи с сервером «ОИК Диспетчер НТ»

## Примеры

См. каталог `samples`

## Используемые функции

#### Функции работы с SVG-элементами на схеме

```
string getElementProperty(string id, string propertyName)

Получает значение свойства элемента с заданным oik:oikid. Возвращает null в случае ошибки

var breakerFill = getElementProperty('breaker', 'fill');
var y = getElementProperty('rectangle', 'y');
```

```
void setElementProperty(string id, string propertyName, string value)

Устанавливает значение свойства элемента с заданным oik:oikid

setElementProperty('breaker', 'fill', 'white');
setElementProperty('value', 'text', getTmAnalogValueString(ti1));
```

#### Фунции работы с телеметрией


```
int initTmStatus(int ch, int rtu, int point)

Инициализирует сигнал с адресом канал:КП:объект и возвращает идентификатор для использования в последующих функциях

var ts1 = initTmStatus(0, 1, 1); // #TC0:1:1
```

```
int initTmAnalog(int ch, int rtu, int point)

Инициализирует измерение с адресом канал:КП:объект и возвращает идентификатор для использования в последующих функциях

var ti1 = initTmAnalog(0, 1, 1); // #TT0:1:1
```

```
int getTmStatusStatus(int id)

Возвращает состояние инициализированного ранее сигнала. 0 - откл, 1 - вкл

var s = getTmStatusStatus(ts1);
```

```
string getTmStatusCaption(int id)

Возвращает текстовое состояние инициализированного ранее сигнала

setElementProperty('value', 'text', getTmStatusCaption(ts1)); // может вернуть, например, "ВЗВЕДЁН"
```

```
bool isTmStatusOn(int id)

Возвращает, является ли инициализированный ранее сигнала включенным

if (isTmStatusOn(ts1)) { ... }
```

```
bool isTmStatusUnreliable(int id)

Возвращает, является ли состояние инициализированного ранее сигнала неактуальным

if (isTmStatusUnreliable(ts1)) { ... }
```

```
bool isTmStatusMalfunction(int id)

Возвращает, является ли состояние инициализированного ранее сигнала неисправным

if (isTmStatusMalfunction(ts1)) { ... }
```

```
bool isTmStatusIntermediate(int id)

Возвращает, является ли состояние инициализированного ранее сигнала промежуточным

if (isTmStatusIntermediate(ts1)) { ... }
```

```
bool isTmStatusManuallySet(int id)

Возвращает, является ли состояние инициализированного ранее сигнала установленным вручную

if (isTmStatusManuallySet(ts1)) { ... }
```

```
bool isTmStatusManuallyBlocked(int id)

Возвращает, является ли состояние инициализированного ранее сигнала заблокированным вручную

if (isTmStatusManuallyBlocked(ts1)) { ... }
```

```
bool isTmStatusManuallyAbnormal(int id)

Возвращает, является ли состояние инициализированного ранее сигнала отклонением от нормы

if (isTmStatusManuallyAbnormal(ts1)) { ... }
```

```
bool isTmStatusUnacked(int id)

Возвращает, является ли состояние инициализированного ранее сигнала неквитированным

if (isTmStatusUnacked(ts1)) { ... }
```

```
bool isTmStatusUnderMaintenance(int id)

Возвращает, является ли положение тележки инициализированного ранее сигнала ремонтным

if (isTmStatusUnderMaintenance(ts1)) { ... }
```

```
bool isTmStatusUnderCheckup(int id)

Возвращает, является ли положение тележки инициализированного ранее сигнала контрольным

if (isTmStatusUnderCheckup(ts1)) { ... }
```

```
bool isTmStatusCustomFlagRaised(int id, int level)

Возвращает, взведён ли произвольный флаг (номер от 1 до 4) инициализированного ранее сигнала

if (isTmStatusCustomFlagRaised(ts1, 1)) { ... }
```

```
string getTmStatusCustomFlagCaption(int id, int level)

Возвращает текстовое состояние произвольного флага (номер от 1 до 4) инициализированного ранее сигнала

setElementProperty('flag1', 'text', getTmStatusCustomFlagCaption(ts1, 1)); // может вернуть, например, "ремонт"
```

```
float getTmAnalogValue(int id)

Возвращает значение инициализированного ранее измерения

var value1 = getTmAnalogValue(ti1);
```

```
string getTmAnalogUnit(int id)

Возвращает единицу измерения инициализированного ранее измерения

setElementProperty('unit', 'text', getTmAnalogUnit(ti1)); // может вернуть, например, "МВт"
```

```
string getTmAnalogValueString(int id)

Возвращает текстовое значение инициализированного ранее измерения с учётом количества знаков после запятой

setElementProperty('value', 'text', getTmAnalogValueString(ti1)); // может вернуть, например, "35,6"
```

```
string getTmAnalogValueWithUnitString(int id)

Возвращает текстовое значение инициализированного ранее измерения с учётом количества знаков после запятой и единицы измерения

setElementProperty('value', 'text', getTmAnalogValueWithUnitString(ti1)); // может вернуть, например, "35,6 МВт"
```

```
bool isTmAnalogUnreliable(int id)

Возвращает, является ли значение инициализированного ранее измерения неактуальным

if (isTmAnalogUnreliable(ti1)) { ... }
```

```
bool isTmAnalogManuallySet(int id)

Возвращает, является ли значение инициализированного ранее измерения установленным вручную

if (isTmAnalogManuallySet(ti1)) { ... }
```

```
bool isTmAnalogManuallyBlocked(int id)

Возвращает, является ли значение инициализированного ранее измерения заблокированным вручную

if (isTmAnalogManuallyBlocked(ti1)) { ... }
```

```
bool isTmAnalogManuallyAbnormal(int id)

Возвращает, является ли значение инициализированного ранее измерения отклонением от нормы

if (isTmAnalogManuallyAbnormal(ti1)) { ... }
```

```
bool isTmAnalogUnacked(int id)

Возвращает, является ли значение инициализированного ранее измерения неквитированным

if (isTmAnalogUnacked(ti1)) { ... }
```

```
bool isTmAnalogAlarmAlert(int id)

Возвращает, взведена ли аварийная уставка инициализированного ранее измерения

if (isTmAnalogAlarmAlert(ti1)) { ... }
```

```
bool isTmAnalogAlarmWarning(int id)

Возвращает, взведена ли предупредительная уставка инициализированного ранее измерения

if (isTmAnalogAlarmWarning(ti1)) { ... }
```

```
bool isTmAnalogAlarmFlagRaised(int id, int level)

Возвращает, взведена ли уставка (уровень важности от 1 до 4) инициализированного ранее измерения

if (isTmAnalogAlarmFlagRaised(ti1, 1)) { ... }
```


#### Функции работы с внутренними переменными

```
int initVariable(string name)

Инициализирует внутреннюю переменную схемы (служат для обмена данными между элементами схемы) и возвращает идентификатор для использования в последующих функциях
Например:
var v1 = initVariable('V1');
```

```
bool isVariableOn(int id)

Возвращает, является ли состояние внутренней переменной включенным

if (isVariableOn(v1)) { ... }
```

```
bool isVariableUnreliable(int id)

Возвращает, является ли состояние внутренней переменной недостоверным

if (isVariableUnreliable(v1)) { ... }
```

```
bool isVariableManuallySet(int id)

Возвращает, является ли состояние внутренней переменной установленным вручную

if (isVariableManuallySet(v1)) { ... }
```

```
void setVariable(int id, bool isOn)

Устанавливает, является ли состояние внутренней переменной включенным

setVariable(v1, true);
setVariable(v1, false);
```

```
void setVariableUnreliable(int id, bool isUnreliable)

Устанавливает, является ли состояние внутренней переменной недостоверным

setVariableUnreliable(v1, true);
setVariableUnreliable(v1, false);
```

```
void setVariableManuallySet(int id, bool isManuallySet)

Устанавливает, является ли состояние внутренней переменной установленным вручную

setVariableManuallySet(v1, true);
setVariableManuallySet(v1, false);
```

#### Функция обработки событий

```
void initCustomToolTip(string text)

Задаёт произвольный текст всплывающего окна при наведении на элемент схемы

initCustomToolTip('Масляный насос горизонтальный одноступенчатый');
```

```
void initTmStatusDefaultCommands(int id)

Создаёт базовый набор команд по нажатию на элемент и в контекстном меню элемента, для инициализированного ранее сигнала

initTmStatusDefaultCommands(ts1);
```

```
void initOpenDocumentDefaultCommands(string id)

Создаёт базовый набор команд по нажатию на элемент и в контекстном меню элемента, для документа с заданным идентификатором

initOpenDocumentDefaultCommands('688e8e28-b51e-43b4-a8b0-9480a7ecef34');
```

```
void initClickIntCommand(ElementCommandType type, int id)

Создаёт команду по нажатию на элемент с целочисленным аргументом (обычно используется для инициализированных ранее сигналов и измерений)
Список типов команд представлен ниже

initClickIntCommand(ACK_TM_STATUS, ts1);
```

```
void initContextMenuIntCommand(ElementCommandType type, int id)

Добавляет команду в контекстное меню элемента с целочисленным аргументом (обычно используется для инициализированных ранее сигналов и измерений)
Список типов команд представлен ниже

initContextMenuIntCommand(OPEN_TM_ANALOG_EVENTS_ARCHIVE, ti1);
initContextMenuIntCommand(OPEN_TM_ANALOG_CHART, ti1);
initContextMenuIntCommand(OPEN_TM_ANALOG_REPORT, ti1);
initContextMenuIntCommand(COPY_TM_ANALOG_TO_CLIPBOARD, ti1);
```

```
void initClickStringCommand(ElementCommandType type, string id)

Создаёт команду по нажатию на элемент со строковым аргументом (обычно используется для идентификатора документа)
Список типов команд представлен ниже

initClickStringCommand(OPEN_DOCUMENT_IN_OVERVIEW, '688e8e28-b51e-43b4-a8b0-9480a7ecef34');
```

```
void initContextMenuStringCommand(ElementCommandType type, int id)

Добавляет команду в контекстное меню элемента со строковым аргументом (обычно используется для идентификатора документа)
Список типов команд представлен ниже

initClickStringCommand(OPEN_DOCUMENT_IN_THIS_TAB, '688e8e28-b51e-43b4-a8b0-9480a7ecef34');
initClickStringCommand(OPEN_DOCUMENT_IN_NEW_TAB, '688e8e28-b51e-43b4-a8b0-9480a7ecef34');
```

```
void initContextMenuSeparator()

Добавляет полосу-разделитель в контекстное меню элемента

initClickStringCommand(OPEN_DOCUMENT_IN_THIS_TAB, '688e8e28-b51e-43b4-a8b0-9480a7ecef34');
initContextMenuSeparator();
initClickStringCommand(OPEN_DOCUMENT_IN_NEW_TAB, '688e8e28-b51e-43b4-a8b0-9480a7ecef34');
```

#### Поддерживаемые типы команд
```
OPEN_DOCUMENT_IN_THIS_TAB         - открыть документ в этой вкладке
OPEN_DOCUMENT_IN_NEW_TAB          - открыть документ в новой вкладке
OPEN_DOCUMENT_IN_UNIQUE_TAB       - открыть документ в уникальной вкладке
OPEN_DOCUMENT_IN_OVERVIEW         - открыть документ во всплывающем окне
START_PROCESS                     - запустить процесс
OPEN_VIDEO_IN_OVERVIEW            - открыть видео во всплывающем окне
SHOW_TM_STATUS                    - показать окно сигнала
TELECONTROL                       - подать команду ТУ (через окно сигнала)
SWITCH_TM_STATUS_MANUALLY         - переключить сигнал вручную
ACK_TM_STATUS                     - квитировать сигнал
OPEN_TM_STATUS_EVENTS_ARCHIVE     - открыть архив событий по сигналу
OPEN_TM_STATUS_CHART              - открыть график по сигналу
OPEN_TM_STATUS_REPORT             - открыть отчет по сигналу
COPY_TM_STATUS_TO_CLIPBOARD       - копировать ТМ-адрес сигнала в буфер обмена
COPY_TM_STATUS_NAME_TO_CLIPBOARD  - копировать наименование сигнала в буфер обмена
ADD_TM_STATUS_TO_QUICK_LIST       - добавить сигнал в быстрый список
SHOW_TM_ANALOG                    - показать окно измерение
TELEREGULATE                      - подать команду телерегулирования (через окно измерения)
SET_TM_ANALOG_MANUALLY            - установить значение измерения вручную
OPEN_TM_ANALOG_TECH_PROPERTIES    - открыть окно технических параметров измерения
OPEN_TM_ANALOG_ALARMS             - открыть окно уставок измерения
OPEN_TM_ANALOG_EVENTS_ARCHIVE     - открыть архив событий по измерению
OPEN_TM_ANALOG_CHART              - открыть график по измерению
OPEN_TM_ANALOG_REPORT             - открыть отчет по измерению
COPY_TM_ANALOG_TO_CLIPBOARD       - копировать ТМ-адрес измерения в буфер обмена
COPY_TM_ANALOG_NAME_TO_CLIPBOARD  - копировать наименование измерения в буфер обмена
ADD_TM_ANALOG_TO_QUICK_LIST       - добавить измерение в быстрый список
```

#### Вспомогательные функции

```
void logMessage(string message)

Выводит отладочное сообщение в консоль программы

logMessage(getTmAnalogValue(ti1));
```

```
void overrideUpdateInterval(int interval)

Устанавливает период обновления текущих данных на схеме, в миллисекундах

overrideUpdateInterval(10000); // раз в 10 секунд
```

```
void overrideTickInterval(int interval)

Устанавливает период вызова функции tick() на схеме, в миллисекундах. По умолчанию раз в секунду (значение 1000)

overrideUpdateInterval(250); // 4 раза в секунду
```