# 🔹 Controller → View → JS 封裝元件規範

## 1️⃣ Controller 層

責任：提供資料和事件接口

- 資料提供：Controller 只負責拿資料（DTO / ViewModel / JSON），不處理 UI 或 DOM。

- 格式統一：返回的資料格式固定，例如：

```csharp
public class MyComponentDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
}
```

- 錯誤處理：Controller 返回狀態碼或訊息，JS 只做提示，不再處理資料邏輯。

- 可配置性：提供必要參數給前端，例如頁碼、篩選條件。

## 2️⃣ View / Partial View 層

責任：渲染結構與初始資料

- HTML 結構：元件的 DOM 範本，盡量使用 Partial View 或 Razor Component 封裝。

- CSS 樣式：可 scoped 或 class 前綴，避免污染全局。

- 初始化資料：可透過 @Model 或 data-attribute 給 JS 初始資料。

```html
<div id="myComponent" data-initial='@Json.Serialize(Model.Items)'></div>
```

## 3️⃣ JavaScript 層

責任：元件行為、互動、AJAX

資料呈現：把 Controller 提供的資料渲染到 DOM。

事件處理：按鈕、表單、互動行為都在 JS 處理。

AJAX / Fetch：需要與 Controller 通訊時，JS 負責發送請求，更新元件狀態。

狀態管理：局部元件狀態封裝在 JS，避免污染全局。

回調接口：元件提供事件回調，供外部頁面使用：

```js
myComponent.onChange = function(selected) { ... }
```

## 4️⃣ 資料流簡圖
```css
Controller (提供資料/DTO)
       ↓
Partial View / Razor Component (HTML + 初始資料)
       ↓
JavaScript (互動、事件、AJAX)
       ↑
Controller (AJAX 請求回應)
```

## 5️⃣ 元件封裝建議

| 元件內容 |	建議 |
| ----- | -------- |
|HTML template | HTML template Partial View，DOM 結構固定 |
|CSS |	元件專屬 class 或 scoped，避免全局污染|
|JS |	模組化封裝，管理互動與狀態|
|初始化資料	|  由 Controller 提供，可透過 @Model 或 data-attribute|
|AJAX|	JS 與 Controller 交互，Controller 僅提供資料接口|
|事件回調	|元件對外提供回調接口，避免耦合頁面邏輯|

---

## 💡 重點：
- 範圍是 從 Controller 到 JS，不管 Repository 或 DB。
- Controller 提供乾淨資料，View 呈現結構和初始資料，JS 處理互動、事件和 AJAX。
- 封裝元件時，保持責任清晰，方便重用和測試。

## 使用文件的方法
1. 直接用這份文件，讓 AI 產一個 Controller → View → JS 封裝元件範本。 (會給一個基礎骨架)
2. 已經有 codebase，依照這份文件去評估是否有違規的部分


