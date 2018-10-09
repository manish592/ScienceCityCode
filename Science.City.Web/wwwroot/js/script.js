
 $.sidebarMenu($('.sidebar-menu'))


$(document).on('click','.toggle-sidebar', function(){
	$('.left-sidebar').toggleClass('move-side-bar');
	$('.right-side-panel').toggleClass('move-right-panel');
})


/////////////////chart-js//////////////////////
var ctx = document.getElementById("myChart").getContext('2d');
var myChart = new Chart(ctx, {
    type: 'line',
    data: {
        labels: ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP"],
        datasets: [{
            label: 'Name-1',
            data: [0,5,10,8, 9, 6, 5, 2, 3],
            backgroundColor: [
                'rgba(92, 187, 240, .6)',
            ],
            borderColor: [
                'rgba(92, 187, 240, .6)',
            ],
        },
        {
            label: 'Name-2',
            data: [10,7,8,13, 10, 4, 5, 2, 1],
            backgroundColor: [
                'rgba(243, 122, 99, .6)'
            ],
            borderColor: [
                'rgba(243, 122, 99, .6)'
            ],
        }]
    },
    options: {
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero:true
                }
            }],
             xAxes: [{
                ticks: {
                    beginAtZero:true
                }
            }]
        }
    }
});