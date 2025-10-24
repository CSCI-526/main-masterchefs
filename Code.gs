function updateChartData() {
  const ss = SpreadsheetApp.getActiveSpreadsheet();
  const responseSheet = ss.getSheetByName('Form Responses 1');
  const chartSheet = ss.getSheetByName('Chart Data');

  // Get all the raw data from the responses
  // Assumes Column B is Level Name and Column C is Completion Time
  const data = responseSheet.getRange(2, 3, responseSheet.getLastRow() - 1, 2).getValues();

  // An object to hold the aggregated data
  const levelStats = {}; // e.g., { "Level 1": { sum: 150, count: 3 } }

  // Loop through all responses and calculate sums and counts
  data.forEach(row => {
    const levelName = row[0];
    const time = parseFloat(row[1]);
    if (!levelStats[levelName]) {
      levelStats[levelName] = { sum: 0, count: 0 };
    }
    levelStats[levelName].sum += time;
    levelStats[levelName].count++;
  });

  // Prepare the final array for the chart sheet
  const result = [['Level Name', 'Average Time (seconds)']];
  for (const levelName in levelStats) {
    const avg = levelStats[levelName].sum / levelStats[levelName].count;
    result.push([levelName, avg]);
  }

  // Clear the old chart data and write the new data
  chartSheet.clear();
  chartSheet.getRange(1, 1, result.length, result[0].length).setValues(result);
}