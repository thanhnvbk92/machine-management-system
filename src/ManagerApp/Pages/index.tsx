import Head from 'next/head'

export default function Home() {
  return (
    <>
      <Head>
        <title>Machine Management System</title>
        <meta name="description" content="Hệ thống quản lý máy móc" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <link rel="icon" href="/favicon.ico" />
      </Head>
      <main className="min-h-screen bg-gray-100 py-6 flex flex-col justify-center sm:py-12">
        <div className="relative py-3 sm:max-w-xl sm:mx-auto">
          <div className="absolute inset-0 bg-gradient-to-r from-blue-300 to-blue-600 shadow-lg transform -skew-y-6 sm:skew-y-0 sm:-rotate-6 sm:rounded-3xl"></div>
          <div className="relative px-4 py-10 bg-white shadow-lg sm:rounded-3xl sm:p-20">
            <div className="max-w-md mx-auto">
              <div className="divide-y divide-gray-200">
                <div className="py-8 text-base leading-6 space-y-4 text-gray-700 sm:text-lg sm:leading-7">
                  <h1 className="text-3xl font-bold text-gray-900 mb-8">Machine Management System</h1>
                  <p>Hệ thống quản lý máy móc cho nhà máy sản xuất.</p>
                  <ul className="list-disc space-y-2">
                    <li className="flex items-start">
                      <span className="text-blue-600 mr-2">•</span>
                      <span>Quản lý máy móc và thiết bị</span>
                    </li>
                    <li className="flex items-start">
                      <span className="text-blue-600 mr-2">•</span>
                      <span>Theo dõi trạng thái hoạt động</span>
                    </li>
                    <li className="flex items-start">
                      <span className="text-blue-600 mr-2">•</span>
                      <span>Quản lý logs và báo cáo</span>
                    </li>
                  </ul>
                </div>
                <div className="pt-6 text-base leading-6 font-bold sm:text-lg sm:leading-7">
                  <p>Sẵn sàng để bắt đầu?</p>
                  <a href="/dashboard" className="text-blue-600 hover:text-blue-800">
                    Đi tới Dashboard &rarr;
                  </a>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </>
  )
}